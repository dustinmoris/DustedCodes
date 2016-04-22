using System;
using System.Collections.Generic;
using System.Runtime.Caching;
using System.Threading.Tasks;
using DustedCodes.Core.Caching;

namespace DustedCodes.Core.Analytics
{
    public class CachedGoogleAnalyticsClient : IGoogleAnalyticsClient
    {
        private readonly IGoogleAnalyticsClient _googleAnalyticsClient;
        private readonly ICache _cache;

        public CachedGoogleAnalyticsClient(
            IGoogleAnalyticsClient googleAnalyticsClient, 
            ICache cache)
        {
            _googleAnalyticsClient = googleAnalyticsClient;
            _cache = cache;
        }

        public async Task<IEnumerable<PageResult>> GetTrendingPagesAsync(byte maxCount)
        {
            const string cacheKey = "GoogleAnalyticsTrendingPages";
            var cachedResult = _cache.Get<IEnumerable<PageResult>>(cacheKey);

            if (cachedResult != null)
                return cachedResult;

            // Don't need to worry about concurrent writes to the cache
            var result = await _googleAnalyticsClient.GetTrendingPagesAsync(maxCount).ConfigureAwait(false);

            _cache.Set(cacheKey, result, new CacheItemPolicy { AbsoluteExpiration = DateTimeOffset.Now.AddHours(24) });

            return result;
        }
    }
}