using System;
using System.Collections.Generic;
using System.Runtime.Caching;
using System.Threading.Tasks;
using DustedCodes.Core.Caching;
using Guardo;

namespace DustedCodes.Core.Data.StaticFileStorage
{
    public sealed class CachedArticleRepository : IArticleRepository
    {
        private readonly IArticleRepository _articleRepository;
        private readonly ICache _cache;

        public static Func<CacheItemPolicy> CreateDefaultCacheItemPolicy = () => new CacheItemPolicy
            {
                AbsoluteExpiration = DateTimeOffset.Now.AddHours(24)
            };

        public CachedArticleRepository(IArticleRepository articleRepository, ICache cache)
        {
            _articleRepository = articleRepository;
            _cache = cache;
        }

        public async Task<Article> GetAsync(string id)
        {
            Requires.NotNullOrEmpty(id, nameof(id));

            var cacheKey = $"Article:{id}";
            var cachedArticle = _cache.Get<Article>(cacheKey);

            if (cachedArticle != null)
                return cachedArticle;

            var article = await _articleRepository.GetAsync(id).ConfigureAwait(false);
            _cache.Set(cacheKey, article, CreateDefaultCacheItemPolicy.Invoke());
            return article;
        }

        public async Task<IEnumerable<Article>> GetOrderedByDateAsync()
        {
            const string cacheKey = "Articles:All-Sorted-By-Date";
            var cachedArticles = _cache.Get<ICollection<Article>>(cacheKey);

            if (cachedArticles != null)
                return cachedArticles;

            var articles = await _articleRepository.GetOrderedByDateAsync().ConfigureAwait(false);
            _cache.Set(cacheKey, articles, CreateDefaultCacheItemPolicy.Invoke());
            return articles;
        }
    }
}