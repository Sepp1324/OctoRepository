﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using OctoAwesome.Threading;

namespace OctoAwesome.Pooling
{
    public sealed class Pool<T> : IPool<T> where T : IPoolElement, new()
    {
        private static readonly Func<T> getInstance;

        private readonly Stack<T> _internalStack;
        private readonly LockSemaphore _semaphoreExtended;

        static Pool()
        {
            var body = Expression.New(typeof(T));
            getInstance = Expression.Lambda<Func<T>>(body).Compile();
        }

        public Pool()
        {
            _internalStack = new();
            _semaphoreExtended = new(1, 1);
        }

        public T Get()
        {
            T obj;

            using (_semaphoreExtended.Wait())
            {
                obj = _internalStack.Count > 0 ? _internalStack.Pop() : getInstance();
            }

            obj.Init(this);
            return obj;
        }

        public void Push(T obj)
        {
            using (_semaphoreExtended.Wait())
            {
                _internalStack.Push(obj);
            }
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