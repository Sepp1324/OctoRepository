using OctoAwesome.EntityComponents;
using OctoAwesome.Logging;
using OctoAwesome.Notifications;
using OctoAwesome.Threading;
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
<<<<<<< HEAD
        private readonly ConcurrentQueue<CacheItem> _unreferencedItems = new ConcurrentQueue<CacheItem>();
        private readonly AutoResetEvent _autoResetEvent = new AutoResetEvent(false);
        private readonly Dictionary<Index3, CacheItem> _cache;
        private readonly Queue<CacheItem> _newChunks;
        private readonly Queue<CacheItem> _oldChunks;
        private readonly CancellationTokenSource _tokenSource;
        private readonly IResourceManager _resourceManager;
        private readonly LockSemaphore _semaphore = new LockSemaphore(1, 1);
        private readonly LockSemaphore _updateSemaphore = new LockSemaphore(1, 1);
        private readonly Task _cleanupTask;
        private readonly ILogger _logger;
        private readonly IEnumerable<(Guid Id, PositionComponent Component)> _positionComponents;
        private IUpdateHub _updateHub;
        
        /// <summary>
        /// Event for changes in the ChunkCOlumn
        /// </summary>
        public event EventHandler<IChunkColumn> ChunkColumnChanged;
=======

        public event EventHandler<IChunkColumn> ChunkColumnChanged;

        private readonly ConcurrentQueue<CacheItem> _unreferencedItems = new ConcurrentQueue<CacheItem>();
        private readonly AutoResetEvent _autoResetEvent = new AutoResetEvent(false);
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
        private readonly LockSemaphore semaphore = new LockSemaphore(1, 1);
        private readonly LockSemaphore updateSemaphore = new LockSemaphore(1, 1);

        // TODO: Früher oder später nach draußen auslagern
        private readonly Task cleanupTask;
        private readonly ILogger logger;
        private readonly IEnumerable<(Guid Id, PositionComponent Component)> positionComponents;
        private IUpdateHub updateHub;
>>>>>>> feature/performance

        /// <summary>
        /// Gibt die Anzahl der aktuell geladenen Chunks zurück.
        /// </summary>
        public int LoadedChunkColumns
        {
            get
            {
                using (_semaphore.Wait())
                    return _cache.Count;
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
<<<<<<< HEAD
            _resourceManager = resourceManager ?? throw new ArgumentNullException(nameof(resourceManager));

            _cache = new Dictionary<Index3, CacheItem>();
            _newChunks = new Queue<CacheItem>();
            _oldChunks = new Queue<CacheItem>();

            _tokenSource = new CancellationTokenSource();
            _cleanupTask = new Task(async () => await BackgroundCleanup(_tokenSource.Token), TaskCreationOptions.LongRunning);
            _cleanupTask.Start(TaskScheduler.Default);
            _logger = (TypeContainer.GetOrNull<ILogger>() ?? NullLogger.Default).As(typeof(GlobalChunkCache));

            var ids = resourceManager.GetEntityIdsFromComponent<PositionComponent>().ToList();
            _positionComponents = resourceManager.GetEntityComponents<PositionComponent>(ids);
=======
            this.resourceManager = resourceManager ?? throw new ArgumentNullException(nameof(resourceManager));

            cache = new Dictionary<Index3, CacheItem>();
            newChunks = new Queue<CacheItem>();
            oldChunks = new Queue<CacheItem>();

            tokenSource = new CancellationTokenSource();
            cleanupTask = new Task(async () => await BackgroundCleanup(tokenSource.Token), TaskCreationOptions.LongRunning);
            cleanupTask.Start(TaskScheduler.Default);
            logger = (TypeContainer.GetOrNull<ILogger>() ?? NullLogger.Default).As(typeof(GlobalChunkCache));

            var ids = resourceManager.GetEntityIdsFromComponent<PositionComponent>().ToList();
            positionComponents = resourceManager.GetEntityComponents<PositionComponent>(ids);
>>>>>>> feature/performance
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

            using (_semaphore.Wait())
            {
<<<<<<< HEAD
                if (!_cache.TryGetValue(new Index3(position, Planet.Id), out cacheItem))
=======

                if (!cache.TryGetValue(new Index3(position, Planet.Id), out cacheItem))
>>>>>>> feature/performance
                {

                    cacheItem = new CacheItem()
                    {
                        Planet = Planet,
                        Index = position,
                        References = 0,
                        ChunkColumn = null,
                    };

                    cacheItem.Changed += ItemChanged;
                    _cache.Add(new Index3(position, Planet.Id), cacheItem);
                }
                cacheItem.References++;

                if (cacheItem.References > 1)
<<<<<<< HEAD
                    _logger.Warn($"Add Reference to:{cacheItem.Index}, now at:{cacheItem.References}");
=======
                    logger.Warn($"Add Reference to:{cacheItem.Index}, now at:{cacheItem.References}");

>>>>>>> feature/performance
            }

            using (cacheItem.Wait())
            {

                if (cacheItem.ChunkColumn == null)
                {
                    cacheItem.ChunkColumn = _resourceManager.LoadChunkColumn(Planet, position);
                    var chunkIndex = new Index3(position, Planet.Id);
<<<<<<< HEAD
                    var loadedEntities = _positionComponents
                        .Where(x => x.Component.Planet == Planet && x.Component.Position.ChunkIndex.X == chunkIndex.X && x.Component.Position.ChunkIndex.Y == chunkIndex.Y)
                        .Select(x => _resourceManager.LoadEntity(x.Id))
=======
                    var loadedEntities = positionComponents
                        .Where(x => x.Component.Planet == Planet && x.Component.Position.ChunkIndex.X == chunkIndex.X && x.Component.Position.ChunkIndex.Y == chunkIndex.Y)
                        .Select(x => resourceManager.LoadEntity(x.Id))
>>>>>>> feature/performance
                        .ToArray();

                    foreach (var entity in loadedEntities)
                        cacheItem.ChunkColumn.Add(entity);

                    using (_updateSemaphore.Wait())
                        _newChunks.Enqueue(cacheItem);
                }

                return cacheItem.ChunkColumn;
            }
        }

<<<<<<< HEAD
        public bool IsChunkLoaded(Index2 position) => _cache.ContainsKey(new Index3(position, Planet.Id));
=======
        public bool IsChunkLoaded(Index2 position)
            => cache.ContainsKey(new Index3(position, Planet.Id));
>>>>>>> feature/performance

        private void ItemChanged(CacheItem obj, IChunkColumn chunkColumn)
        {
            _autoResetEvent.Set();
            ChunkColumnChanged?.Invoke(this, chunkColumn);
        }

        /// <summary>
        /// Liefert den Chunk, sofern geladen.
        /// </summary>
        /// <param name="planet">Die Id des Planeten</param>
        /// <param name="position">Die Position des zurückzugebenden Chunks</param>
        /// <returns>Chunk Instanz oder null, falls nicht geladen</returns>
<<<<<<< HEAD
        public IChunkColumn Peek(Index2 position) => _cache.TryGetValue(new Index3(position, Planet.Id), out CacheItem cacheItem) ? cacheItem.ChunkColumn : null;
=======
        public IChunkColumn Peek(Index2 position)
        {
            if (cache.TryGetValue(new Index3(position, Planet.Id), out CacheItem cacheItem))
                return cacheItem.ChunkColumn;

            return null;
        }
>>>>>>> feature/performance


        /// <summary>
        /// Löscht den gesamten Inhalt des Caches.
        /// </summary>
        public void Clear()
        {
            using (_semaphore.Wait())
            {
<<<<<<< HEAD
                foreach (CacheItem value in _cache.Values)
=======
                foreach (CacheItem value in cache.Values)
>>>>>>> feature/performance
                {
                    value.References = 0;
                    _unreferencedItems.Enqueue(value);
                }
            }
            _autoResetEvent.Set();
        }

        /// <summary>
        /// Gibt einen abonnierten Chunk wieder frei.
        /// </summary>
        /// <param name="position">Die Position des freizugebenden Chunks</param>
        public void Release(Index2 position)
        {
            using (_semaphore.Wait())
            {
<<<<<<< HEAD
                if (!_cache.TryGetValue(new Index3(position, Planet.Id), out CacheItem cacheItem))
=======
                if (!cache.TryGetValue(new Index3(position, Planet.Id), out CacheItem cacheItem))
>>>>>>> feature/performance
                {
                    throw new NotSupportedException(string.Format("Kein Chunk für die Position ({0}) im Cache", position));
                }

                if (--cacheItem.References <= 0)
                {
                    if (cacheItem.References < 0)
                        _logger.Warn($"Remove Reference from {cacheItem.Index}, now at: {cacheItem.References}");

                    _unreferencedItems.Enqueue(cacheItem);
                    _autoResetEvent.Set();
                }
            }
        }

        private Task BackgroundCleanup(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                _autoResetEvent.WaitOne();

                while (_unreferencedItems.TryDequeue(out CacheItem ci))
                {
                    if (ci.References <= 0)
                    {
                        var key = new Index3(ci.Index, ci.Planet.Id);

                        using (ci.Wait())
                            ci.Changed -= ItemChanged;

<<<<<<< HEAD
                        using (_semaphore.Wait())
                            _cache.Remove(key);

                        using (_updateSemaphore.Wait())
                            _oldChunks.Enqueue(ci);
                    }
                }
            }
=======
                        using (semaphore.Wait())
                            cache.Remove(key);

                        using (updateSemaphore.Wait())
                            oldChunks.Enqueue(ci);
                    }
                }
            }

>>>>>>> feature/performance
            return Task.CompletedTask;
        }


        public void BeforeSimulationUpdate(Simulation simulation)
        {
            lock (_updateSemaphore)
            {
                //Neue Chunks in die Simulation einpflegen
                while (_newChunks.Count > 0)
                {
<<<<<<< HEAD
                    var chunk = _newChunks.Dequeue();
=======
                    CacheItem chunk = newChunks.Dequeue();
>>>>>>> feature/performance
                    chunk.ChunkColumn.ForEachEntity(simulation.AddEntity);
                }

                //Alte Chunks aus der Siumaltion entfernen
                while (_oldChunks.Count > 0)
                {
<<<<<<< HEAD
                    using (var chunk = _oldChunks.Dequeue())
=======
                    using (CacheItem chunk = oldChunks.Dequeue())
                    {
>>>>>>> feature/performance
                        chunk.ChunkColumn.ForEachEntity(simulation.RemoveEntity);
                }
            }
        }

        public void AfterSimulationUpdate(Simulation simulation)
        {
            //TODO: Überarbeiten
            using (_semaphore.Wait())
            {
<<<<<<< HEAD
                var failChunkEntities = _cache
=======
                FailEntityChunkArgs[] failChunkEntities = cache
>>>>>>> feature/performance
                    .Where(chunk => chunk.Value.ChunkColumn != null)
                    .SelectMany(chunk => chunk.Value.ChunkColumn.FailChunkEntity())
                    .ToArray();

                foreach (FailEntityChunkArgs entity in failChunkEntities)
                {
<<<<<<< HEAD
                    var currentChunk = Peek(entity.CurrentChunk);
                    var targetChunk = Peek(entity.TargetChunk);
=======
                    IChunkColumn currentchunk = Peek(entity.CurrentChunk);
                    IChunkColumn targetchunk = Peek(entity.TargetChunk);
>>>>>>> feature/performance

                    currentChunk?.Remove(entity.Entity);

                    if (targetChunk != null)
                    {
                        targetChunk.Add(entity.Entity);
                    }
                    else
                    {
                        targetChunk = _resourceManager.LoadChunkColumn(entity.CurrentPlanet, entity.TargetChunk);

                        simulation.RemoveEntity(entity.Entity); //Because we add it again through the targetchunk
                        targetChunk.Add(entity.Entity);
                    }
                }
            }
        }

        public void OnCompleted() { }

<<<<<<< HEAD
        public void OnError(Exception error) => throw error;
=======
        public void OnError(Exception error)
            => throw error;
>>>>>>> feature/performance

        public void OnNext(Notification value)
        {
            switch (value)
            {
                case BlockChangedNotification blockChangedNotification:
                    Update(blockChangedNotification);
                    break;
                case BlocksChangedNotification blocksChangedNotification:
                    Update(blocksChangedNotification);
                    break;
                default:
                    break;
            }
        }

        public void OnUpdate(SerializableNotification notification)
        {
            _updateHub?.Push(notification, DefaultChannels.Network);

            if (notification is IChunkNotification)
                _updateHub?.Push(notification, DefaultChannels.Chunk);
        }

        public void Update(SerializableNotification notification)
        {
<<<<<<< HEAD
            if (notification is IChunkNotification chunk && _cache.TryGetValue(new Index3(chunk.ChunkPos.X, chunk.ChunkPos.Y, chunk.Planet), out var cacheItem)) 
=======
            if (notification is IChunkNotification chunk
                && cache.TryGetValue(new Index3(chunk.ChunkPos.X, chunk.ChunkPos.Y, chunk.Planet),
                out CacheItem cacheItem))
            {
>>>>>>> feature/performance
                cacheItem.ChunkColumn?.Update(notification);
        }

<<<<<<< HEAD
        public void InsertUpdateHub(IUpdateHub updateHub) => this._updateHub = updateHub;
=======
        public void InsertUpdateHub(IUpdateHub updateHub)
            => this.updateHub = updateHub;
>>>>>>> feature/performance

        public void Dispose()
        {
            foreach (var item in _unreferencedItems.ToArray())
                item.Dispose();

<<<<<<< HEAD
            foreach (var item in _cache.ToArray())
                item.Value.Dispose();

            foreach (var item in _newChunks.ToArray())
                item.Dispose();

            foreach (var item in _oldChunks.ToArray())
                item.Dispose();

            _cache.Clear();
            _newChunks.Clear();
            _oldChunks.Clear();

            _semaphore.Dispose();
            _updateSemaphore.Dispose();
=======
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
>>>>>>> feature/performance
            _autoResetEvent.Dispose();
        }

        /// <summary>
        /// Element for the Cache
        /// </summary>
        private class CacheItem : IDisposable
        {
            private IChunkColumn _chunkColumn;
<<<<<<< HEAD
            private readonly LockSemaphore _internalSemaphore;
=======
            private readonly LockSemaphore internalSemaphore;
>>>>>>> feature/performance

            public IPlanet Planet { get; set; }

            public Index2 Index { get; set; }

            /// <summary>
            /// Count of the subscribed Item
            /// </summary>
            public int References { get; set; }

            /// <summary>
            /// Chunk referenced by <see cref="CacheItem"/>
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

<<<<<<< HEAD
            private bool _disposed;

            public CacheItem() => _internalSemaphore = new LockSemaphore(1, 1);

            public LockSemaphore.SemaphoreLock Wait() => _internalSemaphore.Wait();
=======
            private bool disposed;

            public CacheItem() => internalSemaphore = new LockSemaphore(1, 1);

            public LockSemaphore.SemaphoreLock Wait()
                => internalSemaphore.Wait();
>>>>>>> feature/performance

            public void Dispose()
            {
                if (_disposed)
                    return;

                _disposed = true;

                _internalSemaphore.Dispose();

                if (_chunkColumn is IDisposable disposable)
                    disposable.Dispose();

                _chunkColumn = null;
                Planet = null;
            }

<<<<<<< HEAD
            private void OnChanged(IChunkColumn chunkColumn, IChunk chunk) => Changed?.Invoke(this, chunkColumn);
=======
            private void OnChanged(IChunkColumn chunkColumn, IChunk chunk)
                => Changed?.Invoke(this, chunkColumn);

>>>>>>> feature/performance
        }

    }
}
