using System.Collections.Generic;
using System.Threading.Tasks;
using DustedCodes.Blog.Data;

namespace DustedCodes.Blog.Services
{
    public interface IArticleService
    {
        Task<int> GetTotalPageCount();

        Task<IEnumerable<Article>> GetMostRecentAsync(int page);

        Task<IEnumerable<Article>> FindByTagAsync(string tag);

        Task<Article> FindByIdAsync(string id);
    }
}