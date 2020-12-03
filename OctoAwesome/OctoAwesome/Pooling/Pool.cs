using System;
using System.Collections.Generic;
using OctoAwesome.Threading;

namespace OctoAwesome.Pooling
{
    public sealed class Pool<T> : IPool<T> where T : IPoolElement, new()
    {
        private readonly Stack<T> _internalStack;
        private readonly LockSemaphore _lockSemaphore;

        public Pool()
        {
            _internalStack = new Stack<T>();
            _lockSemaphore = new LockSemaphore(1, 1);
        }

        public T Get()
        {
            T obj;

            using (_lockSemaphore.Wait())
            {
                obj = _internalStack.Count > 0 ? _internalStack.Pop() : new T();
            }

            obj.Init(this);
            return obj;
        }

        public void Push(T obj)
        {
            using (_lockSemaphore.Wait())
                _internalStack.Push(obj);
        }

        public void Push(IPoolElement obj)
        {
            if (obj is T t)
                Push(t);
            else
                throw new InvalidCastException("Can not push object from type: " + obj.GetType());
        }
    }
}
