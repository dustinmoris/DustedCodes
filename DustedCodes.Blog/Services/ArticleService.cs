using System.Collections.Generic;
using System.Threading.Tasks;
using DustedCodes.Blog.Data;

namespace DustedCodes.Blog.Services
{
    public sealed class ArticleService : IArticleService
    {
        private readonly IArticleRepository _articleRepository;

        public ArticleService(IArticleRepository articleRepository)
        {
            _articleRepository = articleRepository;
        }

        public async Task<int> GetTotalCount()
        {
            return await _articleRepository.GetTotalCount();
        }

        public async Task<IEnumerable<Article>> GetMostRecentAsync(int page, int pageSize)
        {
            return await _articleRepository.GetMostRecentAsync(page, pageSize);
        }

        public async Task<IEnumerable<Article>> FindByTagAsync(string tag)
        {
            return await _articleRepository.FindByTagAsync(tag);
        }

        public async Task<Article> FindByIdAsync(string id)
        {
            return await _articleRepository.FindAsync(id);
        }
    }
}