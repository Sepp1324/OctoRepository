using System;
using System.Collections.Generic;

namespace OctoAwesome.Pooling
{
    public sealed class Pool<T> : IPool<T> where T : IPoolElement, new()
    {
        private readonly Stack<T> _internalStack;
        private readonly SemaphoreExtended _semaphoreExtended;

        public Pool()
        {
            _internalStack = new Stack<T>();
            _semaphoreExtended = new SemaphoreExtended(1, 1);
        }

        public T Get()
        {
            T obj;

            using (_semaphoreExtended.Wait())
            {
                if (_internalStack.Count > 0)
                    obj = _internalStack.Pop();
                else
                    obj = new T();
            }

            obj.Init(this);
            return obj;
        }

        public void Push(T obj)
        {
            using (_semaphoreExtended.Wait())
                _internalStack.Push(obj);
        }

        public void Push(IPoolElement obj)
        {
            if (obj is T t)
            {
                Push(t);
            }
            else
            {
                throw new InvalidCastException("Can not push object from type: " + obj.GetType());
            }
        }
    }
}
