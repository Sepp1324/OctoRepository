using OctoAwesome.Logging;
using OctoAwesome.Threading;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OctoAwesome
{
    /// <summary>
    /// Chunk Cache für lokale Anwendungen.
    /// </summary>
    public class LocalChunkCache : ILocalChunkCache
    {
<<<<<<< HEAD
        private readonly LockSemaphore _semaphore;
        private readonly ILogger _logger;
     
        /// <summary>
        /// Die im lokalen Cache gespeicherten Chunks
        /// </summary>
        private readonly IChunkColumn[] _chunkColumns;
        
        /// <summary>
        /// Größe des Caches in Zweierpotenzen
        /// </summary>
        private readonly int _limit;
        
        /// <summary>
        /// Maske, die die Grösse des Caches markiert
        /// </summary>
        private readonly int _mask;
        
        public readonly LockSemaphore taskSemaphore;

        /// <summary>
        /// Aktueller Planet auf dem sich der Cache bezieht.
        /// </summary>
        public IPlanet Planet { get; private set; }
        
=======
        private readonly LockSemaphore semaphore;
        private readonly LockSemaphore taskSemaphore;

        /// <summary>
        /// Aktueller Planet auf dem sich der Cache bezieht.
        /// </summary>
        public IPlanet Planet { get; private set; }

        public Index2 CenterPosition { get; set; }

        /// <summary>
        /// Referenz auf den Globalen Cache
        /// </summary>
        private readonly IGlobalChunkCache globalCache;

        /// <summary>
        /// Die im lokalen Cache gespeicherten Chunks
        /// </summary>
        private readonly IChunkColumn[] chunkColumns;
        private readonly ILogger logger;

>>>>>>> feature/performance
        /// <summary>
        /// Gibt die Range in Chunks in alle Richtungen an (bsp. Range = 1 bedeutet centraler Block + links uns rechts jeweils 1 = 3)
        /// </summary>
<<<<<<< HEAD
        private readonly int _range;
        
=======
        private int limit;

>>>>>>> feature/performance
        /// <summary>
        /// Task, der bei einem Wechsel des Zentralen Chunks neue nachlädt falls nötig
        /// </summary>
<<<<<<< HEAD
        private Task _loadingTask;
=======
        private int mask;
>>>>>>> feature/performance

        /// <summary>
        /// Token, das angibt, ob der Chûnk-nachlade-Task abgebrochen werden soll
        /// </summary>
        private CancellationTokenSource _cancellationToken;
        
        /// <summary>
        /// Referenz auf den Globalen Cache
        /// </summary>
<<<<<<< HEAD
        private readonly IGlobalChunkCache _globalCache;
=======
        private int range;
        /// <summary>
        /// Task, der bei einem Wechsel des Zentralen Chunks neue nachlädt falls nötig
        /// </summary>
        private Task _loadingTask;
>>>>>>> feature/performance

        public Index2 CenterPosition { get; set; }
        
        /// <summary>
        /// Token, das angibt, ob der Chûnk-nachlade-Task abgebrochen werden soll
        /// </summary>
        private CancellationTokenSource _cancellationToken;
        /// <summary>
        /// Instanziert einen neuen local Chunk Cache.
        /// </summary>
        /// <param name="globalCache">Referenz auf global Chunk Cache</param>
        /// <param name="dimensions">Größe des Caches in Zweierpotenzen</param>
        /// <param name="range">Gibt die Range in alle Richtungen an.</param>
        public LocalChunkCache(IGlobalChunkCache globalCache, int dimensions, int range)
        {
            if (1 << dimensions < (range * 2) + 1)
                throw new ArgumentException("Range too big");

            
<<<<<<< HEAD
            _semaphore = new LockSemaphore(1, 1);
=======
            semaphore = new LockSemaphore(1, 1);
>>>>>>> feature/performance
            taskSemaphore = new LockSemaphore(1, 1);
            Planet = globalCache.Planet;
            
            _globalCache = globalCache;
            _range = range;
            _limit = dimensions;
            _mask = (1 << _limit) - 1;
            _chunkColumns = new IChunkColumn[(_mask + 1) * (_mask + 1)];
            _logger = (TypeContainer.GetOrNull<ILogger>() ?? NullLogger.Default).As(typeof(LocalChunkCache));
        }


        /// <summary>
        /// Setzt den Zentrums-Chunk für diesen lokalen Cache.
        /// </summary>
        /// <param name="planet">Der Planet, auf dem sich der Chunk befindet</param>
        /// <param name="index">Die Koordinaten an der sich der Chunk befindet</param>
        /// <param name="successCallback">Routine die Aufgerufen werden soll, falls das setzen erfolgreich war oder nicht</param>
        public bool SetCenter(Index2 index, Action<bool> successCallback = null)
        {
            using (taskSemaphore.Wait())
            {
                var callerName = new StackFrame(1).GetMethod().Name;
                _logger.Debug($"Set Center from {callerName}");
                CenterPosition = index;

                if (_loadingTask != null && !_loadingTask.IsCompleted)
                {
<<<<<<< HEAD
                    _logger.Debug("Continue with task on index " + index);
=======
                    logger.Debug("Continue with task on index " + index);
>>>>>>> feature/performance
                    _loadingTask = _loadingTask.ContinueWith(_ => InternalSetCenter(_cancellationToken.Token, index, successCallback));
                }
                else
                {
                    _logger.Debug("New task on index " + index);
                    _cancellationToken?.Cancel();
                    _cancellationToken?.Dispose();
                    _cancellationToken = new CancellationTokenSource();
                    _loadingTask = Task.Run(() => InternalSetCenter(_cancellationToken.Token, index, successCallback));
                }
            }
            return true;
        }

        /// <summary>
        /// Interne Methode, in der der zentrale Chunk gesetzt wird. Die Chunks um den Zentrumschunk werden auch nachgeladen falls nötig
        /// </summary>
        /// <param name="token">Token, um zu prüfen, ob die aktualisierung abgeborchen werden soll</param>
        /// <param name="planet">Der Planet, auf dem die Chunks aktualisiert werden sollen</param>
        /// <param name="index">Der ins Zentrum zu setzende Chunk</param>
        /// <param name="successCallback">Routine die Aufgerufen werden soll, falls das setzen erfolgreich war oder nicht</param>
        private void InternalSetCenter(CancellationToken token, Index2 index, Action<bool> successCallback)
        {
            if (Planet == null)
            {
                successCallback?.Invoke(true);
                return;
            }

<<<<<<< HEAD
            var requiredChunkColumns = new List<Index2>();

            for (var x = -_range; x <= _range; x++)
            {
                for (var y = -_range; y <= _range; y++)
                {
                    var local = new Index2(index.X + x, index.Y + y);
=======
            List<Index2> requiredChunkColumns = new List<Index2>();

            for (int x = -range; x <= range; x++)
            {
                for (int y = -range; y <= range; y++)
                {
                    Index2 local = new Index2(index.X + x, index.Y + y);
>>>>>>> feature/performance
                    local.NormalizeXY(Planet.Size);
                    requiredChunkColumns.Add(local);
                }
            }

            // Erste Abbruchmöglichkeit
            if (token.IsCancellationRequested)
            {
                successCallback?.Invoke(false);
                return;
            }

<<<<<<< HEAD
            foreach (var chunkColumnIndex in requiredChunkColumns.OrderBy(c => index.ShortestDistanceXY(c, new Index2(Planet.Size)).LengthSquared()))
            {
                var localX = chunkColumnIndex.X & _mask;
                var localY = chunkColumnIndex.Y & _mask;
                var flatIndex = FlatIndex(localX, localY);
                var chunkColumn = _chunkColumns[flatIndex];

                // Alten Chunk entfernen, falls notwendig

                using (_semaphore.Wait())
=======
            foreach (var chunkColumnIndex in requiredChunkColumns
                                                .OrderBy(c => index.ShortestDistanceXY(c, new Index2(Planet.Size))
                                                .LengthSquared()))
            {
                int localX = chunkColumnIndex.X & mask;
                int localY = chunkColumnIndex.Y & mask;
                int flatIndex = FlatIndex(localX, localY);
                IChunkColumn chunkColumn = chunkColumns[flatIndex];

                // Alten Chunk entfernen, falls notwendig

                using (semaphore.Wait())
>>>>>>> feature/performance
                {
                    if (chunkColumn != null && chunkColumn.Index != chunkColumnIndex)
                    {
                        //logger.Debug($"Remove Chunk: {chunkColumn.Index}, new: {chunkColumnIndex}");
<<<<<<< HEAD
                        _globalCache.Release(chunkColumn.Index);


                        _chunkColumns[flatIndex] = null;
=======
                        globalCache.Release(chunkColumn.Index);


                        chunkColumns[flatIndex] = null;
>>>>>>> feature/performance
                        chunkColumn = null;
                    }
                }

                // Zweite Abbruchmöglichkeit
                if (token.IsCancellationRequested)
                {
                    successCallback?.Invoke(false);
                    return;
                }

<<<<<<< HEAD
                using (_semaphore.Wait())
=======
                using (semaphore.Wait())
>>>>>>> feature/performance
                {
                    // Neuen Chunk laden
                    if (chunkColumn == null)
                    {
<<<<<<< HEAD
                        chunkColumn = _globalCache.Subscribe(new Index2(chunkColumnIndex));

                        if (chunkColumn?.Index != chunkColumnIndex)
                            _logger.Error($"Loaded Chunk Index: {chunkColumn?.Index}, wanted: {chunkColumnIndex} ");
                        if (_chunkColumns[flatIndex] != null)
                            _logger.Error($"Chunk in Array!!: {flatIndex}, on index: {_chunkColumns[flatIndex].Index} ");


                        _chunkColumns[flatIndex] = chunkColumn;
=======
                        chunkColumn = globalCache.Subscribe(new Index2(chunkColumnIndex));

                        if (chunkColumn?.Index != chunkColumnIndex)
                            logger.Error($"Loaded Chunk Index: {chunkColumn?.Index}, wanted: {chunkColumnIndex} ");
                        if (chunkColumns[flatIndex] != null)
                            logger.Error($"Chunk in Array!!: {flatIndex}, on index: {chunkColumns[flatIndex].Index} ");


                        chunkColumns[flatIndex] = chunkColumn;
>>>>>>> feature/performance

                        if (chunkColumn == null)
                        {
                            successCallback?.Invoke(false);
                            return;
                        }
                    }
                }

                // Dritte Abbruchmöglichkeit
                if (token.IsCancellationRequested)
                {
                    successCallback?.Invoke(false);
                    return;
                }
            }

            successCallback?.Invoke(true);
        }


        /// <summary>
        /// Liefert den Chunk an der angegebenen Chunk-Koordinate zurück.
        /// </summary>
        /// <param name="index">Chunk Index</param>
        /// <returns>Instanz des Chunks</returns>
<<<<<<< HEAD
        public IChunk GetChunk(Index3 index) => GetChunk(index.X, index.Y, index.Z);
=======
        public IChunk GetChunk(Index3 index)
            => GetChunk(index.X, index.Y, index.Z);
>>>>>>> feature/performance

        /// <summary>
        /// Liefert den Chunk an der angegebenen Chunk-Koordinate zurück.
        /// </summary>
        /// <param name="x">X Koordinate</param>
        /// <param name="y">Y Koordinate</param>
        /// <param name="z">Z Koordinate</param>
        /// <returns>Instanz des Chunks</returns>
        public IChunk GetChunk(int x, int y, int z)
        {
            if (Planet == null || z < 0 || z >= Planet.Size.Z)
                return null;

            x = Index2.NormalizeAxis(x, Planet.Size.X);
            y = Index2.NormalizeAxis(y, Planet.Size.Y);

<<<<<<< HEAD
            var chunkColumn = _chunkColumns[FlatIndex(x, y)];
=======
            IChunkColumn chunkColumn = chunkColumns[FlatIndex(x, y)];
>>>>>>> feature/performance

            if (chunkColumn != null && chunkColumn.Index.X == x && chunkColumn.Index.Y == y)
                return chunkColumn.Chunks[z];

            return null;
        }

        /// <summary>
        /// Liefert den Block an der angegebenen Block-Koodinate zurück.
        /// </summary>
        /// <param name="index">Block Index</param>
        /// <returns>Die Block-ID an der angegebenen Koordinate</returns>
<<<<<<< HEAD
        public ushort GetBlock(Index3 index) => GetBlock(index.X, index.Y, index.Z);
=======
        public ushort GetBlock(Index3 index)
            => GetBlock(index.X, index.Y, index.Z);
>>>>>>> feature/performance

        /// <summary>
        /// Liefert den Block an der angegebenen Block-Koodinate zurück.
        /// </summary>
        /// <param name="index">Block Index</param>
        /// <returns>Die Block-ID an der angegebenen Koordinate</returns>
        public BlockInfo GetBlockInfo(Index3 index)
        {
<<<<<<< HEAD
            var chunk = GetChunk(index.X >> Chunk.LimitX, index.Y >> Chunk.LimitY, index.Z >> Chunk.LimitZ);
=======
            IChunk chunk = GetChunk(index.X >> Chunk.LimitX, index.Y >> Chunk.LimitY, index.Z >> Chunk.LimitZ);
>>>>>>> feature/performance

            if (chunk != null)
            {
                var flatIndex = Chunk.GetFlatIndex(index);
                var block = chunk.Blocks[flatIndex];
                var meta = chunk.MetaData[flatIndex];

                return new BlockInfo(index, block, meta);
            }

            return default;
        }

        /// <summary>
        /// Liefert den Block an der angegebenen Block-Koodinate zurück.
        /// </summary>
        /// <param name="x">X-Anteil der Koordinate des Blocks</param>
        /// <param name="y">Y-Anteil der Koordinate des Blocks</param>
        /// <param name="z">Z-Anteil der Koordinate des Blocks</param>
        /// <returns>Die Block-ID an der angegebenen Koordinate</returns>
        public ushort GetBlock(int x, int y, int z)
        {
<<<<<<< HEAD
            var chunk = GetChunk(x >> Chunk.LimitX, y >> Chunk.LimitY, z >> Chunk.LimitZ);
            return chunk?.GetBlock(x, y, z) ?? (ushort) 0;
=======
            IChunk chunk = GetChunk(x >> Chunk.LimitX, y >> Chunk.LimitY, z >> Chunk.LimitZ);

            if (chunk != null)
                return chunk.GetBlock(x, y, z);

            return 0;
>>>>>>> feature/performance
        }

        /// <summary>
        /// Überschreibt den Block an der angegebenen Koordinate.
        /// </summary>
        /// <param name="index">Block-Koordinate</param>
        /// <param name="block">Die neue Block-ID.</param>
<<<<<<< HEAD
        public void SetBlock(Index3 index, ushort block) => SetBlock(index.X, index.Y, index.Z, block);
=======
        public void SetBlock(Index3 index, ushort block)
            => SetBlock(index.X, index.Y, index.Z, block);
>>>>>>> feature/performance

        /// <summary>
        /// Überschreibt den Block an der angegebenen Koordinate.
        /// </summary>
        /// <param name="x">X-Anteil der Koordinate des Blocks innerhalb des Chunks</param>
        /// <param name="y">Y-Anteil der Koordinate des Blocks innerhalb des Chunks</param>
        /// <param name="z">Z-Anteil der Koordinate des Blocks innerhalb des Chunks</param>
        /// <param name="block">Die neue Block-ID</param>
        public void SetBlock(int x, int y, int z, ushort block)
        {
            IChunk chunk = GetChunk(x >> Chunk.LimitX, y >> Chunk.LimitY, z >> Chunk.LimitZ);

            chunk?.SetBlock(x, y, z, block);
        }

        /// <summary>
        /// Gibt die Metadaten des Blocks an der angegebenen Koordinate zurück.
        /// </summary>
        /// <param name="x">X-Anteil der Koordinate des Blocks innerhalb des Chunks</param>
        /// <param name="y">Y-Anteil der Koordinate des Blocks innerhalb des Chunks</param>
        /// <param name="z">Z-Anteil der Koordinate des Blocks innerhalb des Chunks</param>
        /// <returns>Die Metadaten des angegebenen Blocks</returns>
        public int GetBlockMeta(int x, int y, int z)
        {
            IChunk chunk = GetChunk(x >> Chunk.LimitX, y >> Chunk.LimitY, z >> Chunk.LimitZ);

            return chunk?.GetBlockMeta(x, y, z) ?? 0;
        }

        /// <summary>
        /// Gibt die Metadaten des Blocks an der angegebenen Koordinate zurück.
        /// </summary>
        /// <param name="index">Block-Koordinate</param>
        /// <returns>Die Metadaten des angegebenen Blocks</returns>
<<<<<<< HEAD
        public int GetBlockMeta(Index3 index) => GetBlockMeta(index.X, index.Y, index.Z);
=======
        public int GetBlockMeta(Index3 index)
            => GetBlockMeta(index.X, index.Y, index.Z);
>>>>>>> feature/performance

        /// <summary>
        /// Ändert die Metadaten des Blockes an der angegebenen Koordinate. 
        /// </summary>
        /// <param name="x">X-Anteil der Koordinate des Blocks innerhalb des Chunks</param>
        /// <param name="y">Y-Anteil der Koordinate des Blocks innerhalb des Chunks</param>
        /// <param name="z">Z-Anteil der Koordinate des Blocks innerhalb des Chunks</param>
        /// <param name="meta">Die neuen Metadaten</param>
        public void SetBlockMeta(int x, int y, int z, int meta)
        {
<<<<<<< HEAD
            var chunk = GetChunk(x >> Chunk.LimitX, y >> Chunk.LimitY, z >> Chunk.LimitZ);
            chunk?.SetBlockMeta(x, y, z, meta);
=======
            IChunk chunk = GetChunk(x >> Chunk.LimitX, y >> Chunk.LimitY, z >> Chunk.LimitZ);

            if (chunk != null)
                chunk.SetBlockMeta(x, y, z, meta);
>>>>>>> feature/performance
        }

        /// <summary>
        /// Ändert die Metadaten des Blockes an der angegebenen Koordinate. 
        /// </summary>
        /// <param name="index">Block-Koordinate</param>
        /// <param name="meta">Die neuen Metadaten</param>
<<<<<<< HEAD
        public void SetBlockMeta(Index3 index, int meta) => SetBlockMeta(index.X, index.Y, index.Z, meta);
=======
        public void SetBlockMeta(Index3 index, int meta)
            => SetBlockMeta(index.X, index.Y, index.Z, meta);
>>>>>>> feature/performance

        /// <summary>
        /// Leert den Cache und gibt sie beim GlobalChunkCache wieder frei
        /// </summary>
        public void Flush()
        {
<<<<<<< HEAD
            for (var i = 0; i < _chunkColumns.Length; i++)
=======
            for (int i = 0; i < chunkColumns.Length; i++)
>>>>>>> feature/performance
            {
                if (_chunkColumns[i] == null)
                    continue;

<<<<<<< HEAD
                var chunkColumn = _chunkColumns[i];
=======
                IChunkColumn chunkColumn = chunkColumns[i];
>>>>>>> feature/performance

                _globalCache.Release(chunkColumn.Index);
                _chunkColumns[i] = null;
            }
        }

        /// <summary>
<<<<<<< HEAD
        /// Gibt einen falchen Index um auf das Array <see cref="_chunkColumns"/> zu zu greiffen
=======
        /// Gibt einen falchen Index um auf das Array <see cref="chunkColumns"/> zu zu greiffen
>>>>>>> feature/performance
        /// </summary>
        /// <param name="x">Die X-Koordinate</param>
        /// <param name="y">Die Y-Koordinate</param>
        /// <returns>Der Abgeflachte index</returns>
<<<<<<< HEAD
        private int FlatIndex(int x, int y) => (((y & (_mask)) << _limit) | ((x & (_mask))));
=======
        private int FlatIndex(int x, int y)
            => (((y & (mask)) << limit) | ((x & (mask))));

>>>>>>> feature/performance
    }
}
