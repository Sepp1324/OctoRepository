﻿using System;

namespace OctoAwesome
{
    public class ChunkCache : IChunkCache
    {
        //private readonly IDictionary<Index3, IChunk> _chunks = new Dictionary<Index3, IChunk>();
        private readonly IChunk[] _chunks;
        private readonly Func<Index3, IChunk> _loadDelegate;
        private readonly Action<Index3, IChunk> _saveDelegate;

        private const int LimitX = 5;
        private const int LimitY = 5;
        //private const int ZLimit = 5;

        private const int XMask = 31;
        private const int YMask = 31;
        private const int ZMask = 31;

        public ChunkCache(Func<Index3, IChunk> loadDelegate, Action<Index3, IChunk> saveDelegate)
        {
            _loadDelegate = loadDelegate;
            _saveDelegate = saveDelegate;

            _chunks = new IChunk[(XMask + 1) * (YMask + 1) * (ZMask + 1)];
        }

        public IChunk Get(Index3 idx)
        {
            return _chunks[FlatIndex(idx.X, idx.Y, idx.Z)];
        }

        public IChunk Get(int x, int y, int z)
        {
            return _chunks[FlatIndex(x, y, z)];
        }

        public void EnsureLoaded(Index3 idx)
        {
            var flat = FlatIndex(idx.X, idx.Y, idx.Z);

            if (_chunks[flat] == null)
                _chunks[flat] = _loadDelegate(idx);
        }
        public void Release(Index3 idx)
        {
            var flat = FlatIndex(idx.X, idx.Y, idx.Z);
            var chunk = _chunks[flat];

            if(chunk != null)
            {
                _saveDelegate(idx, chunk);
                _chunks[flat] = null;
            }
        }

        public void Release(int x, int y, int z)
        {
            var flat = FlatIndex(x, y, z);
            var chunk = _chunks[flat];

            if (chunk != null)
            {
                _saveDelegate(chunk.Index, chunk);
                _chunks[flat] = null;
            }
        }

        public void Flush()
        {
            for(int i = 0; i < _chunks.Length; i++)
            {
                var chunk = _chunks[i];

                if (chunk != null)
                    _saveDelegate(chunk.Index, chunk);
            }
        }

        private int FlatIndex(int x, int y, int z)
        {
            return ((z & (ZMask)) << (LimitX + LimitY)) | ((y & (YMask)) << LimitX) | ((x & (XMask)));
        }
    }
}
