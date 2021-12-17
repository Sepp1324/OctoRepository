using System;
using System.Collections.Generic;
using System.Linq;

namespace OctoAwesome.PoC
{
    public abstract class Cache 
    {
        public abstract Type TypeOfTValue { get; }

        public abstract Type TypeOfTKey { get; }

        public abstract TValue Get<TKey, TValue>(TKey key);

        internal abstract void CleanUp();
    }

    public abstract class Cache<TKey, TValue> : Cache
    {
        private readonly Dictionary<TKey, CacheItem> _valueCache = new();

        public override Type TypeOfTValue => typeof(TValue);

        public override Type TypeOfTKey => typeof(TKey);

        protected TimeSpan ClearTime { get; set; } = TimeSpan.FromMinutes(15);

        protected TValue GetBy(TKey key)
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

        public override Tv Get<Tk, Tv>(Tk key) => (Tv)(object)GetBy((TKey)(object)key);

        internal override void CleanUp()
        {
            for (var i = _valueCache.Count; i >= 0; i--)
            {
                var (key, value) = _valueCache.ElementAt(i);

                if (value.LastAccessTime.Add(ClearTime) < DateTime.Now)
                    _valueCache.Remove(key);
            }
        }

        internal bool Remove(TKey key) => _valueCache.Remove(key);

        internal record CacheItem(DateTime LastAccessTime, TValue Value);
    }
}
