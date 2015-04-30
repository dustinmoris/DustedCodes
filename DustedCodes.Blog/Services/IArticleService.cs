using System.Collections.Generic;
using System.Threading.Tasks;
using DustedCodes.Blog.Data;

namespace DustedCodes.Blog.Services
{
    public interface IArticleService
    {
        Task<IEnumerable<Article>> GetMostRecentAsync(int maxItemCount);

        Task<IEnumerable<Article>> FindByTagAsync(string tag);

        Task<Article> FindByIdAsync(string id);
    }
}