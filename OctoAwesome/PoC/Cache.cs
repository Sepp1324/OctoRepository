using System;
using System.Collections.Generic;

namespace OctoAwesome.PoC
{
    public abstract class Cache<TKey, TValue>
    {
        private readonly Dictionary<TKey, CacheItem> _valueCache = new();

        protected TimeSpan ClearTime { get; set; } = TimeSpan.FromMinutes(15);

        public TValue Get(TKey key)
        {
            if (_valueCache.TryGetValue(key, out var value) && value.LastAccessTime.Add(ClearTime) < DateTime.Now)
            {
                value = value with { LastAccessTime = DateTime.Now };
            }
            else
            {
                var loadedValue = Load(key);
                value = value with { LastAccessTime = DateTime.Now, Value = loadedValue };
            }

            _valueCache[key] = value;

            return value.Value;
        }

        protected abstract TValue Load(TKey key);

        internal bool Remove(TKey key) => _valueCache.Remove(key);

        internal record CacheItem(DateTime LastAccessTime, TValue Value);
    }
}
