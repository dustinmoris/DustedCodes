using System.Collections.Generic;
using System.Threading.Tasks;
using DustedCodes.Core.Collections;
using DustedCodes.Core.Data;

namespace DustedCodes.Core.Services
{
    public interface IArticleService
    {
        Task<Article> GetByIdAsync(string articleId);

        Task<IEnumerable<Article>> GetByTagAsync(string tag);

        Task<IEnumerable<Article>> GetAllAsync();

        Task<PagedCollection<Article>> GetByPageAsync(int pageSize, int page);

        Task<IEnumerable<Article>> GetMostRecentAsync(int maxCount);

        Task<IEnumerable<Article>> GetTrendingAsync();
    }
}