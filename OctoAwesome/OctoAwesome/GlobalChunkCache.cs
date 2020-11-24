﻿using OctoAwesome.EntityComponents;
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

        private readonly ConcurrentQueue<CacheItem> _unreferencedItems = new ConcurrentQueue<CacheItem>();
        private readonly AutoResetEvent _autoResetEvent = new AutoResetEvent(false);

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
        private readonly SemaphoreExtended _semaphore = new SemaphoreExtended(1, 1);
        private readonly SemaphoreExtended _updateSemaphore = new SemaphoreExtended(1, 1);

        // TODO: Früher oder später nach draußen auslagern
        private readonly Task _cleanupTask;
        private readonly ILogger _logger;
        private readonly IEnumerable<(Guid Id, PositionComponent Component)> _positionComponents;
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

            _cache = new Dictionary<Index3, CacheItem>();
            _newChunks = new Queue<CacheItem>();
            _oldChunks = new Queue<CacheItem>();

            _tokenSource = new CancellationTokenSource();
            _cleanupTask = new Task(async () => await BackgroundCleanup(_tokenSource.Token), TaskCreationOptions.LongRunning);
            _cleanupTask.Start(TaskScheduler.Default);
            _logger = (TypeContainer.GetOrNull<ILogger>() ?? NullLogger.Default).As(typeof(GlobalChunkCache));

            var ids = _resourceManager.GetEntityIdsFromComponent<PositionComponent>();
            _positionComponents = _resourceManager.GetEntityComponents<PositionComponent>(ids);
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

                if (!_cache.TryGetValue(new Index3(position, Planet.Id), out cacheItem))
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
                    _logger.Warn($"Add Reference to:{cacheItem.Index}, now at:{cacheItem.References}");
            }

            if (cacheItem.ChunkColumn == null)
            {
                using (cacheItem.Wait())
                {
                    cacheItem.ChunkColumn = _resourceManager.LoadChunkColumn(Planet, position);

                    var chunkIndex = new Index3(position, Planet.Id);
                    var loadedEntities = _positionComponents.Where(x => x.Component.Planet == Planet && x.Component.Position.ChunkIndex.X == chunkIndex.X && x.Component.Position.ChunkIndex.Y == chunkIndex.Y).Select(x => _resourceManager.LoadEntity(x.Id));

                    foreach (var entity in loadedEntities)
                        cacheItem.ChunkColumn.Entities.Add(entity);

                    using (_updateSemaphore.Wait())
                        _newChunks.Enqueue(cacheItem);
                }
            }
            return cacheItem.ChunkColumn;
        }

        public bool IsChunkLoaded(Index2 position) => _cache.ContainsKey(new Index3(position, Planet.Id));

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
        public IChunkColumn Peek(Index2 position)
        {
            if (_cache.TryGetValue(new Index3(position, Planet.Id), out CacheItem cacheItem))
                return cacheItem.ChunkColumn;

            return null;
        }


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
                if (!_cache.TryGetValue(new Index3(position, Planet.Id), out CacheItem cacheItem))
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

                    foreach (var entity in chunk.ChunkColumn.Entities)
                        simulation.AddEntity(entity);
                }

                //Alte Chunks aus der Siumaltion entfernen
                while (_oldChunks.Count > 0)
                {
                    using (var chunk = _oldChunks.Dequeue())
                    {

                        foreach (var entity in chunk.ChunkColumn.Entities)
                            simulation.RemoveEntity(entity);
                    }
                }
            }
        }

        public void AfterSimulationUpdate(Simulation simulation)
        {
            //TODO: Überarbeiten
            using (_semaphore.Wait())
            {
                var failChunkEntities = _cache
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

                    currentChunk.Entities.Remove(entity.Entity);

                    if (targetChunk != null)
                    {
                        targetChunk.Entities.Add(entity.Entity);
                    }
                    else
                    {
                        targetChunk = _resourceManager.LoadChunkColumn(entity.CurrentPlanet, entity.TargetChunk);
                        simulation.RemoveEntity(entity.Entity);
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
            _updateHub?.Push(notification, DefaultChannels.Network);

            if (notification is ChunkNotification chunkNotification)
                _updateHub?.Push(chunkNotification, DefaultChannels.Chunk);
        }

        public void Update(SerializableNotification notification)
        {
            if (notification is ChunkNotification chunkNotification &&
                _cache.TryGetValue(
                    new Index3(chunkNotification.ChunkPos.X, chunkNotification.ChunkPos.Y, chunkNotification.Planet),
                    out var cacheItem))
            {
                cacheItem.ChunkColumn.Update(notification);
            }
        }

        public void InsertUpdateHub(IUpdateHub updateHub) => this._updateHub = updateHub;

        public void Dispose()
        {
            foreach (var item in _unreferencedItems.ToArray())
                item.Dispose();

            foreach (var cacheItem in _cache.ToArray())
                cacheItem.Value.Dispose();

            foreach (var chunkItem in _newChunks.ToArray())
                chunkItem.Dispose();

            foreach (var chunkItem in _oldChunks.ToArray())
                chunkItem.Dispose();

            _cache.Clear();
            _newChunks.Clear();
            _oldChunks.Clear();

            _semaphore.Dispose();
            _updateSemaphore.Dispose();
            _autoResetEvent.Dispose();
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

            private bool _disposed;

            public CacheItem() => internalSemaphore = new SemaphoreExtended(1, 1);

            public SemaphoreExtended.SemaphoreLock Wait() => internalSemaphore.Wait();

            public void Dispose()
            {
                if (_disposed)
                    return;
                _disposed = true;

                internalSemaphore.Dispose();

                if (_chunkColumn is IDisposable disposable)
                    disposable.Dispose();

                _chunkColumn = null;
                Planet = null;
            }

            private void OnChanged(IChunkColumn chunkColumn, IChunk chunk) => Changed?.Invoke(this, chunkColumn);
        }
    }
}
