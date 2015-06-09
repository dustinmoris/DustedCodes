using System.Collections.Generic;
using System.Threading.Tasks;
using DustedCodes.Blog.Data;

namespace DustedCodes.Blog.Services
{
    public interface IArticleService
    {
        Task<int> GetTotalPageCount(int pageSize);

        Task<IEnumerable<Article>> GetMostRecentAsync(int page, int pageSize);

        Task<IEnumerable<Article>> FindByTagAsync(string tag);

        Task<Article> FindByIdAsync(string id);
    }
}