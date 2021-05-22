using OctoAwesome.Threading;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace OctoAwesome.Pooling
{
    public sealed class Pool<T> : IPool<T> where T : IPoolElement, new()
    {
        private static readonly Func<T> _getInstance;

        static Pool()
        {
            var body = Expression.New(typeof(T));
            _getInstance = Expression.Lambda<Func<T>>(body).Compile();
        }

        private readonly Stack<T> _internalStack;
        private readonly LockSemaphore _semaphoreExtended;

        public Pool()
        {
            _internalStack = new Stack<T>();
            _semaphoreExtended = new LockSemaphore(1, 1);
        }

        public T Get()
        {
            T obj;

            using (_semaphoreExtended.Wait())
                obj = _internalStack.Count > 0 ? _internalStack.Pop() : _getInstance();

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
                Push(t);
            else
                throw new InvalidCastException("Can not push object from type: " + obj.GetType());
        }
    }
}
