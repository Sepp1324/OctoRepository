using OctoAwesome.Threading;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace OctoAwesome.Pooling
{
    public sealed class Pool<T> : IPool<T> where T : IPoolElement, new()
    {
<<<<<<< HEAD
        private static readonly Func<T> _getInstance;

        static Pool()
        {
            var body = Expression.New(typeof(T));
            _getInstance = Expression.Lambda<Func<T>>(body).Compile();
        }

        private readonly Stack<T> _internalStack;
        private readonly LockSemaphore _semaphoreExtended;
=======
        private static readonly Func<T> getInstance;
>>>>>>> feature/performance

        static Pool()
        {
<<<<<<< HEAD
            _internalStack = new Stack<T>();
            _semaphoreExtended = new LockSemaphore(1, 1);
=======
            var body = Expression.New(typeof(T));
            getInstance = Expression.Lambda<Func<T>>(body).Compile();
        }

        private readonly Stack<T> internalStack;
        private readonly LockSemaphore semaphoreExtended;

        public Pool()
        {
            internalStack = new Stack<T>();
            semaphoreExtended = new LockSemaphore(1, 1);
>>>>>>> feature/performance
        }

        public T Get()
        {
            T obj;

<<<<<<< HEAD
            using (_semaphoreExtended.Wait())
                obj = _internalStack.Count > 0 ? _internalStack.Pop() : _getInstance();
=======
            using (semaphoreExtended.Wait())
            {
                if (internalStack.Count > 0)
                    obj = internalStack.Pop();
                else
                    obj = getInstance();
            }
>>>>>>> feature/performance

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
<<<<<<< HEAD
                throw new InvalidCastException("Can not push object from type: " + obj.GetType());
=======
            {
                throw new InvalidCastException("Can not push object from type: " + obj.GetType());
            }
>>>>>>> feature/performance
        }
    }
}
