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
        private readonly AutoResetEvent _autoResetEvent = new AutoResetEvent(false);

        private readonly ConcurrentQueue<CacheItem> _unreferencedItems = new ConcurrentQueue<CacheItem>();

        /// <summary>
        /// Dictionary, das alle <see cref="CacheItem"/>s hält.
        /// </summary>
        private readonly Dictionary<Index3, CacheItem> _cache;

        // TODO: Früher oder später nach draußen auslagern
        private readonly Task _cleanupTask;
        private readonly ILogger _logger;
        private readonly Queue<CacheItem> _newChunks;
        private readonly Queue<CacheItem> _oldChunks;
        private readonly (Guid Id, PositionComponent Component)[] _positionComponents;
        private readonly IResourceManager _resourceManager;

        /// <summary>
        /// Objekt, das für die Locks benutzt wird
        /// </summary>
        private readonly LockSemaphore _semaphore = new LockSemaphore(1, 1);

        private readonly CancellationTokenSource _tokenSource;
        private readonly LockSemaphore _updateSemaphore = new LockSemaphore(1, 1);
        private IUpdateHub _updateHub;

        /// <summary>
        /// Create new instance of GlobalChunkCache
        /// </summary>
        /// <param name="planet"></param>
        /// <param name="resourceManager">the current <see cref="IResourceManager"/> to load ressources/></param>
        public GlobalChunkCache(IPlanet planet, IResourceManager resourceManager)
        {
            Planet = planet ?? throw new ArgumentNullException(nameof(planet));
            _resourceManager = resourceManager ?? throw new ArgumentNullException(nameof(resourceManager));

            _cache = new Dictionary<Index3, CacheItem>();
            _newChunks = new Queue<CacheItem>();
            _oldChunks = new Queue<CacheItem>();

            _tokenSource = new CancellationTokenSource();
            _cleanupTask = new Task(async () => await BackgroundCleanup(_tokenSource.Token),
                TaskCreationOptions.LongRunning);
            _cleanupTask.Start(TaskScheduler.Default);
            _logger = (TypeContainer.GetOrNull<ILogger>() ?? NullLogger.Default).As(typeof(GlobalChunkCache));

            var ids = resourceManager.GetEntityIdsFromComponent<PositionComponent>().ToArray();
            _positionComponents = resourceManager.GetEntityComponents<PositionComponent>(ids);
        }

        /// <summary>
        /// Standard Dispose Pattern
        /// </summary>
        public void Dispose()
        {
            foreach (var item in _unreferencedItems.ToArray())
                item.Dispose();

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
            _autoResetEvent.Dispose();
        }

        public event EventHandler<IChunkColumn> ChunkColumnChanged;

        /// <summary>
        /// Gibt die Anzahl der aktuell geladenen Chunks zurück.
        /// </summary>
        public int LoadedChunkColumns
        {
            get
            {
                using (_semaphore.Wait())
                {
                    return _cache.Count;
                }
            }
        }

        /// <summary>
        /// Anzahl der noch nicht gespeicherten ChunkColumns.
        /// </summary>
        public int DirtyChunkColumn => 0;

        public IPlanet Planet { get; }

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
                if (!_cache.TryGetValue(new Index3(position, Planet.Id), out cacheItem))
                {
                    cacheItem = new CacheItem()
                    {
                        Planet = Planet,
                        Index = position,
                        References = 0,
                        ChunkColumn = null
                    };

                    cacheItem.Changed += ItemChanged;
                    _cache.Add(new Index3(position, Planet.Id), cacheItem);
                }

                cacheItem.References++;

                if (cacheItem.References > 1)
                    _logger.Warn($"Add Reference to:{cacheItem.Index}, now at:{cacheItem.References}");
            }

            using (cacheItem.Wait())
            {
                if (cacheItem.ChunkColumn != null) return cacheItem.ChunkColumn;
                
                cacheItem.ChunkColumn = _resourceManager.LoadChunkColumn(Planet, position);
                var chunkIndex = new Index3(position, Planet.Id);

                foreach (var positionComponent in _positionComponents)
                {
                    if (!(positionComponent.Component.Planet == Planet && positionComponent.Component.Position.ChunkIndex.X == chunkIndex.X && positionComponent.Component.Position.ChunkIndex.Y == chunkIndex.Y))
                        continue;

                    cacheItem.ChunkColumn.Add(_resourceManager.LoadEntity(positionComponent.Component.Entity.Id));
                }

                using (_updateSemaphore.Wait())
                    _newChunks.Enqueue(cacheItem);

                return cacheItem.ChunkColumn;
            }
        }

        public bool IsChunkLoaded(Index2 position) => _cache.ContainsKey(new Index3(position, Planet.Id));

        /// <summary>
        /// Liefert den Chunk, sofern geladen.
        /// </summary>
        /// <param name="planet">Die Id des Planeten</param>
        /// <param name="position">Die Position des zurückzugebenden Chunks</param>
        /// <returns>Chunk Instanz oder null, falls nicht geladen</returns>
        public IChunkColumn Peek(Index2 position) => _cache.TryGetValue(new Index3(position, Planet.Id), out var cacheItem) ? cacheItem.ChunkColumn : null;


        /// <summary>
        /// Löscht den gesamten Inhalt des Caches.
        /// </summary>
        public void Clear()
        {
            using (_semaphore.Wait())
            {
                foreach (var value in _cache.Values)
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
                if (!_cache.TryGetValue(new Index3(position, Planet.Id), out var cacheItem))
                {
                    throw new NotSupportedException($"Kein Chunk für die Position ({position}) im Cache");
                }

                if (--cacheItem.References > 0) return;
                if (cacheItem.References < 0)
                    _logger.Warn($"Remove Reference from {cacheItem.Index}, now at: {cacheItem.References}");

                _unreferencedItems.Enqueue(cacheItem);
                _autoResetEvent.Set();
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="simulation"></param>
        public void BeforeSimulationUpdate(Simulation simulation)
        {
            lock (_updateSemaphore)
            {
                //Neue Chunks in die Simulation einpflegen
                while (_newChunks.Count > 0)
                {
                    var chunk = _newChunks.Dequeue();
                    chunk.ChunkColumn.ForEachEntity(simulation.AddEntity);
                }

                //Alte Chunks aus der Siumaltion entfernen
                while (_oldChunks.Count > 0)
                {
                    using (var chunk = _oldChunks.Dequeue())
                    {
                        chunk.ChunkColumn.ForEachEntity(simulation.RemoveEntity);
                    }
                }
            }
        }

        public void AfterSimulationUpdate(Simulation simulation)
        {
            //TODO: Überarbeiten
            // using (_semaphore.Wait())
            // {
            //     var failChunkEntities = _cache
            //         .Where(chunk => chunk.Value.ChunkColumn != null)
            //         .SelectMany(chunk => chunk.Value.ChunkColumn.FailChunkEntity())
            //         .ToArray();
            //
            //     foreach (var entity in failChunkEntities)
            //     {
            //         var currentchunk = Peek(entity.CurrentChunk);
            //         var targetchunk = Peek(entity.TargetChunk);
            //
            //         currentchunk?.Remove(entity.Entity);
            //
            //         if (targetchunk != null)
            //         {
            //             targetchunk.Add(entity.Entity);
            //         }
            //         else
            //         {
            //             targetchunk = _resourceManager.LoadChunkColumn(entity.CurrentPlanet, entity.TargetChunk);
            //
            //             simulation.RemoveEntity(entity.Entity); //Because we add it again through the targetchunk
            //             targetchunk.Add(entity.Entity);
            //         }
            //     }
            // }
        }

        public void OnCompleted()
        {
        }

        public void OnError(Exception error) => throw error;

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
            if (notification is IChunkNotification chunk && _cache.TryGetValue(new Index3(chunk.ChunkPos.X, chunk.ChunkPos.Y, chunk.Planet), out var cacheItem))
                cacheItem.ChunkColumn?.Update(notification);
        }

        public void InsertUpdateHub(IUpdateHub updateHub) => _updateHub = updateHub;

        private void ItemChanged(CacheItem obj, IChunkColumn chunkColumn)
        {
            _autoResetEvent.Set();
            ChunkColumnChanged?.Invoke(this, chunkColumn);
        }

        private Task BackgroundCleanup(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                _autoResetEvent.WaitOne();

                while (_unreferencedItems.TryDequeue(out var ci))
                {
                    if (ci.References > 0) continue;
                    
                    var key = new Index3(ci.Index, ci.Planet.Id);

                    using (ci.Wait())
                        ci.Changed -= ItemChanged;

                    using (_semaphore.Wait())
                        _cache.Remove(key);

                    using (_updateSemaphore.Wait())
                        _oldChunks.Enqueue(ci);
                }
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// Element für den Cache
        /// </summary>
        private class CacheItem : IDisposable
        {
            private readonly LockSemaphore _internalSemaphore;
            private IChunkColumn _chunkColumn;

            private bool _disposed;

            public CacheItem() => _internalSemaphore = new LockSemaphore(1, 1);

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

            public event Action<CacheItem, IChunkColumn> Changed;

            public LockSemaphore.SemaphoreLock Wait() => _internalSemaphore.Wait();

            private void OnChanged(IChunkColumn chunkColumn, IChunk chunk) => Changed?.Invoke(this, chunkColumn);
        }
    }
}