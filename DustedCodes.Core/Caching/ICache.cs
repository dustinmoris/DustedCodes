using System.Runtime.Caching;

namespace DustedCodes.Core.Caching
{
    public interface ICache
    {
        T Get<T>(string key);

        void Set<T>(string key, T value, CacheItemPolicy policy = null);
    }
}