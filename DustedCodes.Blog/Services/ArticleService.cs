using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DustedCodes.Core.Data;

namespace DustedCodes.Blog.Services
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

        public async Task<ICollection<Article>> GetByTagAsync(string tag)
        {
            var articles = await _articleRepository.GetAllOrderedByDateAsync();

            return articles
                .Where(a => a.Metadata != null 
                    && a.Metadata.Tags != null 
                    && a.Metadata.Tags.Contains(tag))
                .ToList();
        }

        public async Task<ICollection<Article>> GetAllAsync()
        {
            return await _articleRepository.GetAllOrderedByDateAsync();
        }

        public async Task<PagedCollection<Article>> GetMostRecentAsync(int pageSize, int page)
        {
            var articles = await _articleRepository.GetAllOrderedByDateAsync();

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
    }
}