using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OctoAwesome
{
    public class ChunkCache : IChunkCache
    {
        private IDictionary<Index3, IChunk> _chunks = new Dictionary<Index3, IChunk>();
        private Func<Index3, IChunk> _loadDelegate;
        private Action<Index3, IChunk> _saveDelegate;

        public ChunkCache(Func<Index3, IChunk> loadDelegate, Action<Index3, IChunk> saveDelegate)
        {
            _loadDelegate = loadDelegate;
            _saveDelegate = saveDelegate;
        }

        public IChunk Get(Index3 idx)
        {
            if (!_chunks.ContainsKey(idx))
                return null;
            return _chunks[idx];
        }

        public void EnsureLoaded(Index3 idx)
        {
            if (!_chunks.ContainsKey(idx))
                _chunks[idx] = _loadDelegate(idx);
        }
        public void Release(Index3 idx)
        {
            IChunk chunk;
            
            if(_chunks.TryGetValue(idx, out chunk))
            {
                _saveDelegate(idx, chunk);
                _chunks.Remove(idx);
            }
        }
    }
}
