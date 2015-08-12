using System.Runtime.Caching;

namespace DustedCodes.Core.Caching
{
    public sealed class NullCache : ICache
    {
        public T Get<T>(string key)
        {
            return default(T);
        }

        public void Set<T>(string key, T value, CacheItemPolicy policy = null)
        {
        }
    }
}