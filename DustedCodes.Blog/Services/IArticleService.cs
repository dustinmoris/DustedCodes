using System.Collections.Generic;
using System.Threading.Tasks;
using DustedCodes.Blog.Data;

namespace DustedCodes.Blog.Services
{
    public interface IArticleService
    {
        Task<Article> GetByIdAsync(string articleId);

        Task<ICollection<Article>> GetByTagAsync(string tag);

        Task<ICollection<Article>> GetAllAsync();

        Task<PagedCollection<Article>> GetMostRecentAsync(int pageSize, int page);
    }
}