using System;

namespace OctoAwesome.Runtime
{
    public class PlanetResourceManager : IPlanetResourceManager
    {
        private readonly IChunkCache _chunkCache;

        internal IChunkCache ChunkCache { get { return _chunkCache; } }

        public PlanetResourceManager(IChunkCache chunkCache)
        {
            _chunkCache = chunkCache;
        }

        public IChunk GetChunk(Index3 index)
        {
            return _chunkCache.Get(index);
        }

        public IBlock GetBlock(int x, int y, int z)
        {
            var chunk = _chunkCache.Get(x, y, z);
            return chunk.GetBlock(x, y, z);
        }

        public void SetBlock(int x, int y, int z, IBlock block)
        {
            var chunk = _chunkCache.Get(x, y, z);
            chunk.SetBlock(x, y, z, block);
        }

        [Obsolete]
        public IBlock GetBlock(Index3 index)
        {
            return GetBlock(index.X, index.Y, index.Z);
        }

        [Obsolete]
        public void SetBlock(Index3 index, IBlock block)
        {
            SetBlock(index.X, index.Y, index.Z, block);
        }
    }
}
