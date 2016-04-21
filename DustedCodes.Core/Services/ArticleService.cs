using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DustedCodes.Core.Analytics;
using DustedCodes.Core.Collections;
using DustedCodes.Core.Data;

namespace DustedCodes.Core.Services
{
    public sealed class ArticleService : IArticleService
    {
        private readonly IArticleRepository _articleRepository;
        private readonly IGoogleAnalyticsClient _googleAnalyticsClient;

        public ArticleService(IArticleRepository articleRepository, IGoogleAnalyticsClient googleAnalyticsClient)
        {
            _articleRepository = articleRepository;
            _googleAnalyticsClient = googleAnalyticsClient;
        }

        public async Task<Article> GetByIdAsync(string articleId)
        {
            return await _articleRepository.GetAsync(articleId).ConfigureAwait(false);
        }

        public async Task<IEnumerable<Article>> GetByTagAsync(string tag)
        {
            var articles = await _articleRepository.GetOrderedByDateAsync().ConfigureAwait(false);

            return articles.Where(a => a.Tags != null && a.Tags.Contains(tag));
        }

        public async Task<IEnumerable<Article>> GetAllAsync()
        {
            return await _articleRepository.GetOrderedByDateAsync().ConfigureAwait(false);
        }

        public async Task<PagedCollection<Article>> GetByPageAsync(int pageSize, int page)
        {
            var articles = await _articleRepository.GetOrderedByDateAsync().ConfigureAwait(false);
            var articleList = articles.ToList();

            var items = articleList
                .Skip(pageSize * (page - 1))
                .Take(pageSize)
                .ToList();

            return new PagedCollection<Article>
            {
                Items = items,
                PageNumber = page,
                PageSize = pageSize,
                TotalItems = articleList.Count,
                // Usig a simple cast as it is unlikely that this number will exceed Int32
                TotalPages = (int)Math.Ceiling((double)articleList.Count / pageSize)
            };
        }

        public async Task<IEnumerable<Article>> GetMostRecentAsync(int maxCount)
        {
            var articles = await _articleRepository.GetOrderedByDateAsync().ConfigureAwait(false);

            return articles.Take(maxCount);
        }

        public async Task<IEnumerable<Article>> GetTrendingAsync()
        {
            var trendingPages = await _googleAnalyticsClient.GetTrendingPagesAsync(byte.MaxValue).ConfigureAwait(false);
            var articles = await _articleRepository.GetOrderedByDateAsync().ConfigureAwait(false);
            var articleList = articles.ToList();
            var top10TrendingArticles = new List<Article>();

            foreach (var page in trendingPages)
            {
                var article = articleList.FirstOrDefault(a => page.Path.ToLower().Contains(a.Id.ToLower()));

                if (article == default(Article))
                    continue;

                top10TrendingArticles.Add(article);

                if (top10TrendingArticles.Count == 10)
                    break;
            }

            return top10TrendingArticles;
        }
    }
}