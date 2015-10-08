using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DustedCodes.Core.Caching;
using DustedCodes.Core.Collections;
using DustedCodes.Core.Data;

namespace DustedCodes.Core.Services
{
    public sealed class ArticleService : IArticleService
    {
        private readonly IArticleRepository _articleRepository;
        private readonly ICache _cache;
        private const string SortedArticlesCacheKey = "sorted_articles";

        public ArticleService(IArticleRepository articleRepository, ICache cache)
        {
            _articleRepository = articleRepository;
            _cache = cache;
        }

        public async Task<Article> GetByIdAsync(string articleId)
        {
            var cachedArticle = _cache.Get<Article>(articleId);

            if (cachedArticle != null)
                return cachedArticle;

            return await _articleRepository.GetAsync(articleId).ConfigureAwait(false);
        }

        public async Task<IEnumerable<Article>> GetByTagAsync(string tag)
        {
            var articles = await GetAllArticlesSortedByDateAsync().ConfigureAwait(false);

            return articles.Where(a => a.Tags != null && a.Tags.Contains(tag));
        }

        public async Task<IEnumerable<Article>> GetAllAsync()
        {
            return await GetAllArticlesSortedByDateAsync().ConfigureAwait(false);
        }

        public async Task<PagedCollection<Article>> GetByPageAsync(int pageSize, int page)
        {
            var articles = await GetAllArticlesSortedByDateAsync().ConfigureAwait(false);

            var items = articles
                .Skip(pageSize * (page - 1))
                .Take(pageSize)
                .ToList();

            return new PagedCollection<Article>
            {
                Items = items,
                PageNumber = page,
                PageSize = pageSize,
                TotalItems = articles.Count,
                // Usig a simple cast as it is unlikely that this number will exceed Int32
                TotalPages = (int)Math.Ceiling((double)articles.Count / pageSize)
            };
        }

        public async Task<IEnumerable<Article>> GetMostRecentAsync(int maxCount)
        {
            var articles = await GetAllArticlesSortedByDateAsync().ConfigureAwait(false);

            return articles.Take(maxCount);
        }

        private async Task<ICollection<Article>> GetAllArticlesSortedByDateAsync()
        {
            var cachedArticles = _cache.Get<ICollection<Article>>(SortedArticlesCacheKey);

            if (cachedArticles != null)
                return cachedArticles;

            var articles = (await _articleRepository.GetOrderedByDateAsync().ConfigureAwait(false)).ToList();
            _cache.Set(SortedArticlesCacheKey, articles);

            return articles;
        }
    }
}