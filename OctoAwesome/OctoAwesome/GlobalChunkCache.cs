using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace OctoAwesome
{
    /// <summary>
    /// Globaler Cache für Chunks
    /// </summary>
    public sealed class GlobalChunkCache : IGlobalChunkCache
    {
        public event EventHandler<IChunk> ChunkChanged;

        private readonly ConcurrentQueue<CacheItem> _dirtyItems = new ConcurrentQueue<CacheItem>();
        private readonly ConcurrentQueue<CacheItem> _unreferencedItems = new ConcurrentQueue<CacheItem>();
        private readonly AutoResetEvent _autoResetEvent = new AutoResetEvent(false);
        /// <summary>
        /// Dictionary, das alle <see cref="CacheItem"/>s hält.
        /// </summary>
        private readonly Dictionary<Index3, CacheItem> _cache;
        private Queue<CacheItem> _newChunks;
        private Queue<CacheItem> _oldChunks;

        private object _updateLockObject = new object();

        /// <summary>
        /// Funktion, die für das Laden der Chunks verwendet wird
        /// </summary>
        private readonly Func<int, Index2, IChunkColumn> _loadDelegate;

        /// <summary>
        /// Routine, die für das Speichern der Chunks verwendet wird.
        /// </summary>
        private readonly Action<int, Index2, IChunkColumn> _saveDelegate;

        private readonly Func<int, IPlanet> _loadPlanetDelagte;

        /// <summary>
        /// Objekt, das für die Locks benutzt wird
        /// </summary>
        private readonly object _lockObject = new object();

        // TODO: Früher oder später nach draußen auslagern
        private Thread _cleanupThread;

        /// <summary>
        /// Gibt die Anzahl der aktuell geladenen Chunks zurück.
        /// </summary>
        public int LoadedChunkColumns
        {
            get
            {
                lock (_lockObject)
                {
                    return _cache.Count;
                }
            }
        }

        /// <summary>
        /// Anzahl der noch nicht gespeicherten ChunkColumns.
        /// </summary>
        public int DirtyChunkColumn => _dirtyItems.Count;

        /// <summary>
        /// Erzeugt eine neue Instaz der Klasse GlobalChunkCache
        /// </summary>
        /// <param name="loadDelegate">Delegat, der nicht geladene ChunkColumns nachläd.</param>
        /// <param name="loadPlanetDelegate"></param>
        /// <param name="saveDelegate">Delegat, der nicht mehr benötigte ChunkColumns abspeichert.</param>
        public GlobalChunkCache(Func<int, Index2, IChunkColumn> loadDelegate, Func<int, IPlanet> loadPlanetDelegate,
            Action<int, Index2, IChunkColumn> saveDelegate)
        {
            _loadDelegate = loadDelegate ?? throw new ArgumentNullException("loadDelegate");
            _saveDelegate = saveDelegate ?? throw new ArgumentNullException("saveDelegate");
            _loadPlanetDelagte = loadPlanetDelegate ?? throw new ArgumentNullException(nameof(loadPlanetDelegate));

            _cache = new Dictionary<Index3, CacheItem>();
            _newChunks = new Queue<CacheItem>();
            _oldChunks = new Queue<CacheItem>();

            _cleanupThread = new Thread(BackgroundCleanup)
            {
                IsBackground = true,
                Priority = ThreadPriority.Lowest
            };
            _cleanupThread.Start();
        }

        /// <summary>
        /// Abonniert einen Chunk.
        /// </summary>
        /// <param name="planet">Die Id des Planeten</param>
        /// <param name="position">Position des Chunks</param>
        /// <returns></returns>
        public IChunkColumn Subscribe(int planet, Index2 position, bool passive)
        {
            CacheItem cacheItem = null;

            lock (_lockObject)
            {
                if (!_cache.TryGetValue(new Index3(position, planet), out cacheItem))
                {
                    //TODO: Überdenken
                    if (passive)
                        return null;

                    cacheItem = new CacheItem()
                    {
                        Planet = planet,
                        Index = position,
                        References = 0,
                        PassiveReference = 0,
                        ChunkColumn = null,
                    };

                    cacheItem.Changed += ItemChanged;
                    //_dirtyItems.Enqueue(cacheItem);
                    _cache.Add(new Index3(position, planet), cacheItem);
                    //_autoResetEvent.Set();
                }

                if (passive)
                    cacheItem.PassiveReference++;
                else
                    cacheItem.References++;
            }

            lock (cacheItem)
            {
                if (cacheItem.ChunkColumn == null)
                {
                    cacheItem.ChunkColumn = _loadDelegate(planet, position);

                    lock (_updateLockObject)
                    {
                        _newChunks.Enqueue(cacheItem);
                    }
                }
            }

            return cacheItem.ChunkColumn;
        }

        public bool IsChunkLoaded(int planet, Index2 position)
            => _cache.ContainsKey(new Index3(position, planet));

        private void ItemChanged(CacheItem obj, IChunk chunk)
        {
            _dirtyItems.Enqueue(obj);
            _autoResetEvent.Set();
            ChunkChanged?.Invoke(this, chunk);
        }

        /// <summary>
        /// Liefert den Chunk, sofern geladen.
        /// </summary>
        /// <param name="planet">Die Id des Planeten</param>
        /// <param name="position">Die Position des zurückzugebenden Chunks</param>
        /// <returns>Chunk Instanz oder null, falls nicht geladen</returns>
        public IChunkColumn Peek(int planet, Index2 position)
        {
            if (_cache.TryGetValue(new Index3(position, planet), out CacheItem cacheItem))
                return cacheItem.ChunkColumn;

            return null;
        }

        /// <summary>
        /// Löscht den gesamten Inhalt des Caches.
        /// </summary>
        public void Clear()
        {
            lock (_lockObject)
            {
                foreach (var value in _cache.Values)
                {
                    value.References = 0;
                    value.PassiveReference = 0;
                    _unreferencedItems.Enqueue(value);
                }
            }
            _autoResetEvent.Set();
        }

        /// <summary>
        /// Gibt einen abonnierten Chunk wieder frei.
        /// </summary>
        /// <param name="planet">Die Id des Planeten</param>
        /// <param name="position">Die Position des freizugebenden Chunks</param>
        public void Release(int planet, Index2 position, bool passive)
        {
            CacheItem cacheItem;
            lock (_lockObject)
            {
                if (!_cache.TryGetValue(new Index3(position, planet), out cacheItem))
                {
                    if (!passive)
                    {
                        throw new NotSupportedException(string.Format("Kein Chunk für die Position ({0}) im Cache", position));
                    }

                    return;
                }

                if (passive)
                {
                    if (cacheItem != null)
                        cacheItem.PassiveReference--;
                }
                else
                {
                    if (--cacheItem.References <= 0)
                    {
                        _unreferencedItems.Enqueue(cacheItem);
                        _autoResetEvent.Set();
                    }
                }
            }
        }

        private void BackgroundCleanup()
        {
            while (true)
            {
                _autoResetEvent.WaitOne();
                CacheItem ci;
                var itemsToSave = new List<CacheItem>();

                while (_dirtyItems.TryDequeue(out ci))
                    itemsToSave.Add(ci);

                foreach (var item in itemsToSave.Distinct())
                {
                    lock (item)
                    {
                        _saveDelegate(item.Planet, item.Index, item.ChunkColumn);
                    }
                }

                while (_unreferencedItems.TryDequeue(out ci))
                {
                    lock (ci)
                    {
                        if (ci.References <= 0)
                        {
                            var key = new Index3(ci.Index, ci.Planet);

                            lock (_lockObject)
                            {
                                ci.Changed -= ItemChanged;
                                _cache.Remove(key);
                            }
                            lock (_updateLockObject)
                            {
                                _oldChunks.Enqueue(ci);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gibt einen Planenten anhand seiner ID zurück
        /// </summary>
        /// <param name="id">ID des Planeten</param>
        /// <returns>Planet</returns>
        public IPlanet GetPlanet(int id)
            => _loadPlanetDelagte(id);

        public void BeforeSimulationUpdate(Simulation simulation)
        {
            lock (_updateLockObject)
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
                    var chunk = _oldChunks.Dequeue();

                    foreach (var entity in chunk.ChunkColumn.Entities)
                        simulation.RemoveEntity(entity);
                }
            }
        }

        public void AfterSimulationUpdate(Simulation simulation)
        {
            //TODO: Überarbeiten
            lock (_lockObject)
            {
                var failChunkEntities = _cache
                    .Where(chunk => chunk.Value.ChunkColumn != null)
                    .SelectMany(chunk => chunk.Value.ChunkColumn.Entities.FailChunkEntity())
                    .ToArray();

                foreach (var entity in failChunkEntities)
                {
                    var currentchunk = Peek(entity.CurrentPlanet, entity.CurrentChunk);
                    var targetchunk = Peek(entity.TargetPlanet, entity.TargetChunk);

                    currentchunk.Entities.Remove(entity.Entity);

                    if (targetchunk != null)
                    {
                        targetchunk.Entities.Add(entity.Entity);
                    }
                    else
                    {
                        targetchunk = _loadDelegate(entity.CurrentPlanet, entity.TargetChunk);
                        targetchunk.Entities.Add(entity.Entity);
                        _saveDelegate(entity.CurrentPlanet, entity.TargetChunk, targetchunk);
                        simulation.RemoveEntity(entity.Entity);
                    }
                }

            }
        }

        /// <summary>
        /// Element für den Cache
        /// </summary>
        private class CacheItem
        {
            private IChunkColumn _chunkColumn;
            public int Planet { get; set; }

            public Index2 Index { get; set; }

            /// <summary>
            /// Die Zahl der Subscriber, die das Item Abboniert hat.
            /// </summary>
            public int References { get; set; }

            public int PassiveReference { get; set; }

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

            public event Action<CacheItem, IChunk> Changed;

            private void OnChanged(IChunkColumn chunkColumn , IChunk chunk, int changeCounter)
                => Changed?.Invoke(this, chunk);
        }
    }
}
