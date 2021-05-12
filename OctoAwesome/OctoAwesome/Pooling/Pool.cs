using OctoAwesome.Threading;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace OctoAwesome.Pooling
{
    public sealed class Pool<T> : IPool<T> where T : IPoolElement, new()
    {
        private static readonly Func<T> getInstance;

        static Pool()
        {
            var body = Expression.New(typeof(T));
            getInstance = Expression.Lambda<Func<T>>(body).Compile();
        }

        private readonly Stack<T> internalStack;
        private readonly LockSemaphore semaphoreExtended;

        public Pool()
        {
            internalStack = new Stack<T>();
            semaphoreExtended = new LockSemaphore(1, 1);
        }

        public T Get()
        {
            T obj;

            using (semaphoreExtended.Wait())
            {
                obj = internalStack.Count > 0 ? internalStack.Pop() : getInstance();
            }

            obj.Init(this);
            return obj;
        }

        public void Push(T obj)
        {
            using (semaphoreExtended.Wait())
                internalStack.Push(obj);
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
