﻿using System;
using System.Linq.Expressions;

namespace OctoAwesome.Database.Expressions
{
    public static class InstanceCreator<T> where T : new()
    {
        static InstanceCreator()
        {
            var body = Expression.New(typeof(T));
            CreateInstance = Expression.Lambda<Func<T>>(body).Compile();
        }

        public static Func<T> CreateInstance { get; }
    }
}