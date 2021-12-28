using System;
using System.Collections.Generic;

namespace OctoAwesome.PoC
{
    public class CacheService
    {
        private readonly DependencyAgent _dependencyAgent;

        private readonly Dictionary<Type, Cache> _caches;

        public CacheService(DependencyAgent dependency)
        {
            _dependencyAgent = dependency;
            _caches = new ();
        }

        public bool AddCache(Cache cache)
        {
            var type = cache.TypeOfTKey;
            return _caches.TryAdd(type, cache);
        }

        public TValue Get<TKey, TValue>(TKey key)
        {
            if (!_caches.TryGetValue(typeof(TValue), out var cache) || cache.TypeOfTKey != typeof(TKey)) 
                return default;

            var types = _dependencyAgent.GetDependencyTypeOrder(key, cache.TypeOfTKey, cache.TypeOfTValue);

            return cache.Get<TKey, TValue>(key);
        }

        //TODO: CleanUp-Task
    }
}
