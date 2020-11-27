using OctoAwesome.EntityComponents;
using OctoAwesome.Logging;
using OctoAwesome.Notifications;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OctoAwesome
{
    /// <summary>
    /// Globaler Cache für Chunks
    /// </summary>
    public sealed class GlobalChunkCache : IGlobalChunkCache, IDisposable
    {

        public event EventHandler<IChunkColumn> ChunkColumnChanged;

        private readonly ConcurrentQueue<CacheItem> unreferencedItems = new ConcurrentQueue<CacheItem>();
        private readonly AutoResetEvent autoResetEvent = new AutoResetEvent(false);
        /// <summary>
        /// Dictionary, das alle <see cref="CacheItem"/>s hält.
        /// </summary>
        private readonly Dictionary<Index3, CacheItem> cache;
        private readonly Queue<CacheItem> newChunks;
        private readonly Queue<CacheItem> oldChunks;
        private readonly CancellationTokenSource tokenSource;
        private readonly IResourceManager resourceManager;

        /// <summary>
        /// Objekt, das für die Locks benutzt wird
        /// </summary>
        private readonly SemaphoreExtended semaphore = new SemaphoreExtended(1, 1);
        private readonly SemaphoreExtended updateSemaphore = new SemaphoreExtended(1, 1);

        // TODO: Früher oder später nach draußen auslagern
        private readonly Task cleanupTask;
        private readonly ILogger logger;
        private readonly IEnumerable<(Guid Id, PositionComponent Component)> positionComponents;
        private IUpdateHub updateHub;
      
        /// <summary>
        /// Gibt die Anzahl der aktuell geladenen Chunks zurück.
        /// </summary>
        public int LoadedChunkColumns
        {
            get
            {
                using (semaphore.Wait())
                {
                    return cache.Count;
                }
            }
        }

        /// <summary>
        /// Anzahl der noch nicht gespeicherten ChunkColumns.
        /// </summary>
        public int DirtyChunkColumn => 0;

        public IPlanet Planet { get; }

        /// <summary>
        /// Create new instance of GlobalChunkCache
        /// </summary>
        /// <param name="resourceManager">the current <see cref="IResourceManager"/> to load ressources/></param>
        public GlobalChunkCache(IPlanet planet, IResourceManager resourceManager)
        {
            Planet = planet ?? throw new ArgumentNullException(nameof(planet));
            this.resourceManager = resourceManager ?? throw new ArgumentNullException(nameof(resourceManager));

            cache = new Dictionary<Index3, CacheItem>();
            newChunks = new Queue<CacheItem>();
            oldChunks = new Queue<CacheItem>();

            tokenSource = new CancellationTokenSource();
            cleanupTask = new Task(async () => await BackgroundCleanup(tokenSource.Token), TaskCreationOptions.LongRunning);
            cleanupTask.Start(TaskScheduler.Default);
            logger = (TypeContainer.GetOrNull<ILogger>() ?? NullLogger.Default).As(typeof(GlobalChunkCache));

            var ids = resourceManager.GetEntityIdsFromComponent<PositionComponent>();
            positionComponents = resourceManager.GetEntityComponents<PositionComponent>(ids);
        }

        /// <summary>
        /// Abonniert einen Chunk.
        /// </summary>
        /// <param name="planet">Die Id des Planeten</param>
        /// <param name="position">Position des Chunks</param>
        /// <returns></returns>
        public IChunkColumn Subscribe(Index2 position)
        {
            CacheItem cacheItem = null;

            using (semaphore.Wait())
            {

                if (!cache.TryGetValue(new Index3(position, Planet.Id), out cacheItem))
                {

                    cacheItem = new CacheItem()
                    {
                        Planet = Planet,
                        Index = position,
                        References = 0,
                        ChunkColumn = null,
                    };

                    cacheItem.Changed += ItemChanged;
                    //_dirtyItems.Enqueue(cacheItem);
                    cache.Add(new Index3(position, Planet.Id), cacheItem);
                    //_autoResetEvent.Set();
                }
                cacheItem.References++;

                if (cacheItem.References > 1)
                    logger.Warn($"Add Reference to:{cacheItem.Index}, now at:{cacheItem.References}");
            }


            if (cacheItem.ChunkColumn == null)
            {
                using (cacheItem.Wait())
                {
                    cacheItem.ChunkColumn = resourceManager.LoadChunkColumn(Planet, position);
                    var chunkIndex = new Index3(position, Planet.Id);
                    var loadedEntities = positionComponents
                        .Where(x => x.Component.Planet == Planet && x.Component.Position.ChunkIndex.X == chunkIndex.X && x.Component.Position.ChunkIndex.Y == chunkIndex.Y)
                        .Select(x=> resourceManager.LoadEntity(x.Id));

                    foreach (var entity in loadedEntities)
                        cacheItem.ChunkColumn.Entities.Add(entity);

                    using (updateSemaphore.Wait())
                        newChunks.Enqueue(cacheItem);
                }
            }
            return cacheItem.ChunkColumn;
        }

        public bool IsChunkLoaded(Index2 position) => cache.ContainsKey(new Index3(position, Planet.Id));

        private void ItemChanged(CacheItem obj, IChunkColumn chunkColumn)
        {
            autoResetEvent.Set();
            ChunkColumnChanged?.Invoke(this, chunkColumn);
        }

        /// <summary>
        /// Liefert den Chunk, sofern geladen.
        /// </summary>
        /// <param name="planet">Die Id des Planeten</param>
        /// <param name="position">Die Position des zurückzugebenden Chunks</param>
        /// <returns>Chunk Instanz oder null, falls nicht geladen</returns>
        public IChunkColumn Peek(Index2 position)
        {
            if (cache.TryGetValue(new Index3(position, Planet.Id), out CacheItem cacheItem))
                return cacheItem.ChunkColumn;

            return null;
        }


        /// <summary>
        /// Löscht den gesamten Inhalt des Caches.
        /// </summary>
        public void Clear()
        {
            using (semaphore.Wait())
            {
                foreach (CacheItem value in cache.Values)
                {
                    value.References = 0;
                    unreferencedItems.Enqueue(value);
                }
            }
            autoResetEvent.Set();
        }

        /// <summary>
        /// Gibt einen abonnierten Chunk wieder frei.
        /// </summary>
        /// <param name="position">Die Position des freizugebenden Chunks</param>
        public void Release(Index2 position)
        {
            using (semaphore.Wait())
            {
                if (!cache.TryGetValue(new Index3(position, Planet.Id), out CacheItem cacheItem))
                {
                    throw new NotSupportedException(string.Format("Kein Chunk für die Position ({0}) im Cache", position));
                }

                if (--cacheItem.References <= 0)
                {
                    if (cacheItem.References < 0)
                        logger.Warn($"Remove Reference from {cacheItem.Index}, now at: {cacheItem.References}");

                    unreferencedItems.Enqueue(cacheItem);
                    autoResetEvent.Set();
                }
            }
        }

        private Task BackgroundCleanup(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                autoResetEvent.WaitOne();

                while (unreferencedItems.TryDequeue(out CacheItem ci))
                {
                    if (ci.References <= 0)
                    {
                        var key = new Index3(ci.Index, ci.Planet.Id);

                        using (ci.Wait())
                            ci.Changed -= ItemChanged;

                        using (semaphore.Wait())
                            cache.Remove(key);

                        using (updateSemaphore.Wait())
                            oldChunks.Enqueue(ci);
                    }
                }
            }
            return Task.CompletedTask;
        }

        public void BeforeSimulationUpdate(Simulation simulation)
        {
            lock (updateSemaphore)
            {
                //Neue Chunks in die Simulation einpflegen
                while (newChunks.Count > 0)
                {
                    var chunk = newChunks.Dequeue();

                    foreach (Entity entity in chunk.ChunkColumn.Entities)
                        simulation.AddEntity(entity);
                }

                //Alte Chunks aus der Siumaltion entfernen
                while (oldChunks.Count > 0)
                {
                    using (var chunk = oldChunks.Dequeue())
                    {
                        foreach (Entity entity in chunk.ChunkColumn.Entities)
                            simulation.RemoveEntity(entity);
                    }
                }
            }
        }

        public void AfterSimulationUpdate(Simulation simulation)
        {
            //TODO: Überarbeiten
            using (semaphore.Wait())
            {
                var failChunkEntities = cache
                    .Where(chunk => chunk.Value.ChunkColumn != null)
                    .SelectMany(chunk => chunk.Value.ChunkColumn.Entities.FailChunkEntity())
                    .ToArray();

                foreach (var entity in failChunkEntities)
                {
                    //TODO: Old Planet change
                    //var currentchunk = Peek(entity.CurrentPlanet, entity.CurrentChunk);
                    //var targetchunk = Peek(entity.TargetPlanet, entity.TargetChunk);
                    
                    var currentChunk = Peek(entity.CurrentChunk);
                    var targetChunk = Peek(entity.TargetChunk);

                    currentChunk?.Entities.Remove(entity.Entity);

                    if (targetChunk != null)
                    {
                        targetChunk.Entities.Add(entity.Entity);
                    }
                    else
                    {
                        targetChunk = resourceManager.LoadChunkColumn(entity.CurrentPlanet, entity.TargetChunk);

                        simulation.RemoveEntity(entity.Entity); //Because we add it again through the targetchunk
                        targetChunk.Entities.Add(entity.Entity);
                    }
                }
            }
        }

        public void OnCompleted() { }

        public void OnError(Exception error) => throw error;

        public void OnNext(Notification value)
        {
            switch (value)
            {
                case ChunkNotification chunkNotification:
                    Update(chunkNotification);
                    break;
                default:
                    break;
            }
        }

        public void OnUpdate(SerializableNotification notification)
        {
            updateHub?.Push(notification, DefaultChannels.Network);

            if (notification is ChunkNotification chunkNotification)
                updateHub?.Push(chunkNotification, DefaultChannels.Chunk);
        }

        public void Update(SerializableNotification notification)
        {
            if (notification is ChunkNotification chunkNotification
                && cache.TryGetValue(
                        new Index3(chunkNotification.ChunkPos.X, chunkNotification.ChunkPos.Y, chunkNotification.Planet),
                        out var cacheItem))
            {
                cacheItem.ChunkColumn.Update(notification);
            }
        }

        public void InsertUpdateHub(IUpdateHub updateHub) => this.updateHub = updateHub;

        public void Dispose()
        {
            foreach (var item in unreferencedItems.ToArray())
                item.Dispose();

            foreach (var item in cache.ToArray())
                item.Value.Dispose();

            foreach (var item in newChunks.ToArray())
                item.Dispose();

            foreach (var item in oldChunks.ToArray())
                item.Dispose();

            cache.Clear();
            newChunks.Clear();
            oldChunks.Clear();

            semaphore.Dispose();
            updateSemaphore.Dispose();
            autoResetEvent.Dispose();
        }

        /// <summary>
        /// Element für den Cache
        /// </summary>
        private class CacheItem : IDisposable
        {
            private IChunkColumn _chunkColumn;
            private readonly SemaphoreExtended internalSemaphore;

            public IPlanet Planet { get; set; }

            public Index2 Index { get; set; }

            /// <summary>
            /// Die Zahl der Subscriber, die das Item Abboniert hat.
            /// </summary>
            public int References { get; set; }


            /// <summary>
            /// Der Chunk, auf den das <see cref="CacheItem"/> referenziert
            /// </summary>
            public IChunkColumn ChunkColumn
            {
                get => _chunkColumn;
                set
                {
                    if (_chunkColumn != null)
                        _chunkColumn.Changed -= OnChanged;

                    _chunkColumn = value;

                    if (value != null)
                        value.Changed += OnChanged;
                }
            }

            public event Action<CacheItem, IChunkColumn> Changed;

            private bool disposed;

            public CacheItem() => internalSemaphore = new SemaphoreExtended(1, 1);

            public SemaphoreExtended.SemaphoreLock Wait()
                => internalSemaphore.Wait();

            public void Dispose()
            {
                if (disposed)
                    return;

                disposed = true;

                internalSemaphore.Dispose();

                if (_chunkColumn is IDisposable disposable)
                    disposable.Dispose();

                _chunkColumn = null;
                Planet = null;
            }

            private void OnChanged(IChunkColumn chunkColumn, IChunk chunk)
                => Changed?.Invoke(this, chunkColumn);

        }

    }
}
