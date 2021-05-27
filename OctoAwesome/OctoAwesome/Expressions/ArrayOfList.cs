using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace OctoAwesome.Expressions
{
    public static class ArrayOfList<T>
    {
        public static readonly Func<List<T>, T[]> GetArray;
        
        static ArrayOfList()
        {
            ParameterExpression input = Expression.Parameter(typeof(List<T>), "list");
            //ParameterExpression result = Expression.Parameter(typeof(T[]), "list");

            var body = Expression.Field(input, "_items");

            GetArray = Expression.Lambda<Func<List<T>, T[]>>(body, input).Compile();
        }
    }
}
