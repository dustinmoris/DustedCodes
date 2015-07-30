using System.Collections.Generic;
using System.Threading.Tasks;
using DustedCodes.Core.Collections;
using DustedCodes.Core.Data;

namespace DustedCodes.Core.Services
{
    public interface IArticleService
    {
        Task<Article> GetByIdAsync(string articleId);

        Task<ICollection<Article>> GetByTagAsync(string tag);

        Task<ICollection<Article>> GetAllAsync();

        Task<PagedCollection<Article>> GetMostRecentAsync(int pageSize, int page);
    }
}