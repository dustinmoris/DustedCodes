using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DustedCodes.Core.Collections;
using DustedCodes.Core.Data;

namespace DustedCodes.Core.Services
{
    public sealed class ArticleService : IArticleService
    {
        private readonly IArticleRepository _articleRepository;

        public ArticleService(IArticleRepository articleRepository)
        {
            _articleRepository = articleRepository;
        }

        public async Task<Article> GetByIdAsync(string articleId)
        {
            return await _articleRepository.GetAsync(articleId);
        }

        public async Task<IEnumerable<Article>> GetByTagAsync(string tag)
        {
            var articles = await _articleRepository.GetAllSortedByDateAsync();

            return articles.Where(a => a.Tags != null && a.Tags.Contains(tag));
        }

        public async Task<IEnumerable<Article>> GetAllAsync()
        {
            return await _articleRepository.GetAllSortedByDateAsync();
        }

        public async Task<PagedCollection<Article>> GetByPageAsync(int pageSize, int page)
        {
            var articles = await _articleRepository.GetAllSortedByDateAsync();

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
            var articles = await _articleRepository.GetAllSortedByDateAsync();

            return articles.Take(maxCount);
        }
    }
}