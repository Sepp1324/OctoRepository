using OctoAwesome.EntityComponents;
using OctoAwesome.Logging;
using OctoAwesome.Notifications;
using OctoAwesome.Pooling;
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
        public event EventHandler<IChunkColumn> ChunkColumnChanged;

        private readonly ConcurrentQueue<CacheItem> _unreferencedItems = new();
        private readonly AutoResetEvent _autoResetEvent = new(false);

        /// <summary>
        /// Dictionary, das alle <see cref="CacheItem"/>s hält.
        /// </summary>
        private readonly Dictionary<Index3, CacheItem> _cache;
        private readonly Queue<CacheItem> _newChunks;
        private readonly Queue<CacheItem> _oldChunks;
        private readonly CancellationTokenSource _tokenSource;
        private readonly IResourceManager _resourceManager;

        /// <summary>
        /// Objekt, das für die Locks benutzt wird
        /// </summary>
        private readonly LockSemaphore _semaphore = new(1, 1);
        private readonly LockSemaphore _updateSemaphore = new(1, 1);

        // TODO: Früher oder später nach draußen auslagern
        private readonly Task _cleanupTask;
        private readonly ILogger _logger;
        private readonly ChunkPool _chunkPool;
        private readonly (Guid Id, PositionComponent Component)[] _positionComponents;
        private IUpdateHub _updateHub;

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
        /// Create new instance of GlobalChunkCache
        /// </summary>
        /// <param name="resourceManager">the current <see cref="IResourceManager"/> to load ressources/></param>
        public GlobalChunkCache(IPlanet planet, IResourceManager resourceManager)
        {
            Planet = planet ?? throw new ArgumentNullException(nameof(planet));
            _resourceManager = resourceManager ?? throw new ArgumentNullException(nameof(resourceManager));

            _cache = new();
            _newChunks = new();
            _oldChunks = new();

            _tokenSource = new();
            _cleanupTask = new(async () => await BackgroundCleanup(_tokenSource.Token), TaskCreationOptions.LongRunning);
            _cleanupTask.Start(TaskScheduler.Default);
            _logger = (TypeContainer.GetOrNull<ILogger>() ?? NullLogger.Default).As(typeof(GlobalChunkCache));

            _chunkPool = TypeContainer.Get<ChunkPool>();

            var ids = resourceManager.GetEntityIdsFromComponent<PositionComponent>().ToArray();
            _positionComponents = resourceManager.GetEntityComponents<PositionComponent>(ids);
        }

        /// <summary>
        /// Subscribes a Chunk
        /// </summary>
        /// <param name="position">Position of the Chunk</param>
        /// <returns></returns>
        public IChunkColumn Subscribe(Index2 position)
        {
            CacheItem cacheItem = null;

            using (_semaphore.Wait())
            {
                if (!_cache.TryGetValue(new(position, Planet.Id), out cacheItem))
                {
                    cacheItem = new(_chunkPool)
                    {
                        Planet = Planet,
                        Index = position,
                        References = 0,
                        ChunkColumn = null,
                    };

                    cacheItem.Changed += ItemChanged;
                    //_dirtyItems.Enqueue(cacheItem);
                    _cache.Add(new Index3(position, Planet.Id), cacheItem);
                    //_autoResetEvent.Set();
                }
                cacheItem.References++;

                if (cacheItem.References > 1)
                    _logger.Warn($"Add Reference to:{cacheItem.Index}, now at:{cacheItem.References}");
            }

            using (cacheItem.Wait())
            {
                if (cacheItem.ChunkColumn == null)
                {
                    cacheItem.ChunkColumn = _resourceManager.LoadChunkColumn(Planet, position);
                    var chunkIndex = new Index3(position, Planet.Id);

                    foreach (var positionComponent in _positionComponents)
                    {
                        if (!(positionComponent.Component.Planet == Planet && positionComponent.Component.Position.ChunkIndex.X == chunkIndex.X && positionComponent.Component.Position.ChunkIndex.Y == chunkIndex.Y))
                            continue;

                        if (positionComponent.Component.Instance is Entity e)
                            cacheItem.ChunkColumn.Add(_resourceManager.LoadEntity(e.Id));
                    }

                    using (_updateSemaphore.Wait())
                        _newChunks.Enqueue(cacheItem);
                }

                return cacheItem.ChunkColumn;
            }
        }

        /// <summary>
        /// Returns whether Chunk is loaded or not
        /// </summary>
        /// <param name="position">Chunk-Position</param>
        /// <returns></returns>
        public bool IsChunkLoaded(Index2 position) => _cache.ContainsKey(new(position, Planet.Id));

        private void ItemChanged(CacheItem obj, IChunkColumn chunkColumn)
        {
            _autoResetEvent.Set();
            ChunkColumnChanged?.Invoke(this, chunkColumn);
        }

        /// <summary>
        /// Returns Chunk if loaded
        /// </summary>
        /// <param name="position">Position of Chunk</param>
        /// <returns>If loaded <see cref="IChunkColumn"/>, else null</returns>
        public IChunkColumn Peek(Index2 position) => _cache.TryGetValue(new(position, Planet.Id), out var cacheItem) ? cacheItem.ChunkColumn : null;

        /// <summary>
        /// Deletes Values of Cache
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
                    throw new NotSupportedException($"No Chunk for Position ({position}) in Cache!");
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

                while (_unreferencedItems.TryDequeue(out var ci))
                {
                    if (ci.References <= 0)
                    {
                        var key = new Index3(ci.Index, ci.Planet.Id);

                        using (ci.Wait())
                            ci.Changed -= ItemChanged;

                        using (_semaphore.Wait())
                            _cache.Remove(key);

                        using (_updateSemaphore.Wait())
                            _oldChunks.Enqueue(ci);
                    }
                }
            }

            return Task.CompletedTask;
        }


        public void BeforeSimulationUpdate(Simulation simulation)
        {
            lock (_updateSemaphore)
            {
                //Neue Chunks in die Simulation einpflegen
                while (_newChunks.Count > 0)
                {
                    var chunk = _newChunks.Dequeue();
                    chunk.ChunkColumn.ForEachEntity(simulation.Add);
                }

                //Alte Chunks aus der Siumaltion entfernen
                while (_oldChunks.Count > 0)
                {
                    using CacheItem chunk = _oldChunks.Dequeue();
                    chunk.ChunkColumn.ForEachEntity(simulation.Remove);
                }
            }
        }

        public void AfterSimulationUpdate(Simulation simulation)
        {
            //TODO: Überarbeiten
            //using (semaphore.Wait())
            //{
            //    FailEntityChunkArgs[] failChunkEntities = cache
            //        .Where(chunk => chunk.Value.ChunkColumn != null)
            //        .SelectMany(chunk => chunk.Value.ChunkColumn.FailChunkEntity())
            //        .ToArray();

            //    foreach (FailEntityChunkArgs entity in failChunkEntities)
            //    {
            //        IChunkColumn currentchunk = Peek(entity.CurrentChunk);
            //        IChunkColumn targetchunk = Peek(entity.TargetChunk);

            //        currentchunk?.Remove(entity.Entity);

            //        if (targetchunk != null)
            //        {
            //            targetchunk.Add(entity.Entity);
            //        }
            //        else
            //        {
            //            targetchunk = resourceManager.LoadChunkColumn(entity.CurrentPlanet, entity.TargetChunk);

            //            simulation.RemoveEntity(entity.Entity); //Because we add it again through the targetchunk
            //            targetchunk.Add(entity.Entity);
            //        }
            //    }
            //}
        }

        public void OnCompleted() { }

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
            _updateHub?.Push(notification, DefaultChannels.NETWORK);

            if (notification is IChunkNotification)
                _updateHub?.Push(notification, DefaultChannels.CHUNK);
        }

        public void Update(SerializableNotification notification)
        {
            if (notification is IChunkNotification chunk && _cache.TryGetValue(new(chunk.ChunkPos.X, chunk.ChunkPos.Y, chunk.Planet), out var cacheItem))
                cacheItem.ChunkColumn?.Update(notification);
        }

        public void InsertUpdateHub(IUpdateHub updateHub) => _updateHub = updateHub;

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

        /// <summary>
        /// Element for the Cache
        /// </summary>
        private class CacheItem : IDisposable
        {
            private readonly ChunkPool _chunkPool;
            private IChunkColumn _chunkColumn;
            private readonly LockSemaphore _internalSemaphore;

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

            private bool _disposed;

            public CacheItem(ChunkPool chunkPool)
            {
                _internalSemaphore = new(1, 1);

                _chunkPool = chunkPool;
            }

            public LockSemaphore.SemaphoreLock Wait() => _internalSemaphore.Wait();

            public void Dispose()
            {
                if (_disposed)
                    return;

                _disposed = true;

                _internalSemaphore.Dispose();

                foreach (var chunk in _chunkColumn.Chunks)
                {
                    _chunkPool.Push(chunk);
                }

                if (_chunkColumn is IDisposable disposable)
                    disposable.Dispose();

                _chunkColumn = null;
                Planet = null;
            }
            private void OnChanged(IChunkColumn chunkColumn, IChunk chunk) => Changed?.Invoke(this, chunkColumn);
        }
    }
}