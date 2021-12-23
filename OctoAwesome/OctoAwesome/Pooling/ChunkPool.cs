using System;
using System.Collections.Generic;
using OctoAwesome.Threading;

namespace OctoAwesome.Pooling
{
    public sealed class ChunkPool : IPool<Chunk>
    {
        private readonly Stack<Chunk> _internalStack;
        private readonly LockSemaphore _semaphoreExtended;

        public ChunkPool()
        {
            _internalStack = new();
            _semaphoreExtended = new(1, 1);
        }

        [Obsolete("Can not be used. Use Get(Index3, IPlanet) instead.", true)]
        public Chunk Get() => throw new NotSupportedException("Use Get(Index3, IPlanet) instead.");


        public void Push(Chunk obj)
        {
            using (_semaphoreExtended.Wait())
            {
                _internalStack.Push(obj);
            }
        }

        public void Push(IPoolElement obj)
        {
            if (obj is Chunk chunk)
                Push(chunk);
            else
                throw new InvalidCastException("Can not push object from type: " + obj.GetType());
        }

        public Chunk Get(Index3 position, IPlanet planet)
        {
            Chunk obj;

            using (_semaphoreExtended.Wait())
            {
                obj = _internalStack.Count > 0 ? _internalStack.Pop() : new(position, planet);
            }

            obj.Init(position, planet);
            return obj;
        }
    }
}