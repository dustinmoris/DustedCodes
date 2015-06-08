using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DustedCodes.Blog.Data
{
    public sealed class LocalArticleRepository : IArticleRepository
    {
        private readonly IArticleCache _articleCache;
        private readonly IArticleReader _articleReader;

        public LocalArticleRepository(IArticleCache articleCache, IArticleReader articleReader)
        {
            _articleCache = articleCache;
            _articleReader = articleReader;
        }

        public async Task<Article> FindAsync(string articleId)
        {
            if (string.IsNullOrEmpty(articleId))
                throw new ArgumentException("articleId cannot be null or empty.");

            await CheckAndFillCacheAsync();

            var metadata = _articleCache.Metadata.SingleOrDefault(a => a.Id.Equals(articleId));

            if (metadata == default(ArticleMetadata))
                return null;

            return await _articleReader.ReadAsync(metadata.Id);
        }

        public async Task<IEnumerable<Article>> GetMostRecentAsync(int page, int pageSize)
        {
            await CheckAndFillCacheAsync();

            var mostRecentArticles = _articleCache
                .Metadata.OrderByDescending(a => a.PublishDateTime)
                .Skip((page - 1) * pageSize)
                .Take(pageSize);

            return await ReadArticlesAsync(mostRecentArticles);
        }

        public async Task<IEnumerable<Article>> FindByTagAsync(string tag)
        {
            if (string.IsNullOrEmpty(tag))
                throw new ArgumentException("tag cannot be null or empty.");

            await CheckAndFillCacheAsync();

            var taggedArticles = _articleCache.Metadata
                .Where(a => a.Tags != null && a.Tags.Contains(tag))
                .OrderByDescending(a => a.PublishDateTime);

            return await ReadArticlesAsync(taggedArticles);
        }

        private async Task CheckAndFillCacheAsync()
        {
            if (_articleCache.Metadata != null)
                return;

            var articles = await _articleReader.ReadAllAsync();

            _articleCache.Metadata = new List<ArticleMetadata>(
                articles.Select(a => a.Metadata));
        }

        private async Task<IEnumerable<Article>> ReadArticlesAsync(IEnumerable<ArticleMetadata> references)
        {
            var articles = new List<Article>();

            foreach (var metadata in references)
            {
                var article = await _articleReader.ReadAsync(metadata.Id);
                articles.Add(article);
            }

            return articles;
        }
    }
}