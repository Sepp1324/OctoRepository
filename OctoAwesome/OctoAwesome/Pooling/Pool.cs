using OctoAwesome.Threading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.Pooling
{
    public sealed class Pool<T> : IPool<T> where T : IPoolElement, new()
    {
        private readonly Stack<T> _internalStack;
        private readonly LockSemaphore _semaphoreExtended;
        private readonly Func<T> GetInstance;

        public Pool()
        {
            _internalStack = new Stack<T>();
            _semaphoreExtended = new LockSemaphore(1, 1);

            var body = Expression.New(typeof(T));

            GetInstance = Expression.Lambda<Func<T>>(body).Compile();
        }

        public T Get()
        {
            T obj;

            using (_semaphoreExtended.Wait())
            {
                obj = _internalStack.Count > 0 ? _internalStack.Pop() : new T();
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
                Push(t);
            else
                throw new InvalidCastException("Cannot push object from type: " + obj.GetType());
        }
    }
}