using System.Runtime.Caching;

namespace DustedCodes.Core.Caching
{
    public sealed class ObjectCacheWrapper : ICache
    {
        private readonly ObjectCache _objectCache;
        private readonly CacheItemPolicy _defaultCacheItemPolicy;

        public ObjectCacheWrapper(ObjectCache objectCache, CacheItemPolicy defaultCacheItemPolicy)
        {
            _objectCache = objectCache;
            _defaultCacheItemPolicy = defaultCacheItemPolicy;
        }

        public T Get<T>(string key)
        {
            // Returns null if the key does not exist.
            return (T)_objectCache[key];
        }

        public void Set<T>(string key, T value, CacheItemPolicy policy = null)
        {
            _objectCache.Set(key, value, policy ?? _defaultCacheItemPolicy);
        }
    }
}