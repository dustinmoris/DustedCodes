using System.Collections.Generic;
using System.Threading.Tasks;

namespace DustedCodes.Blog.Data
{
    public interface IArticleRepository
    {
        Task<Article> FindAsync(string articleId);

        Task<IEnumerable<Article>> GetMostRecentAsync(int maxCount);

        Task<IEnumerable<Article>> FindByTagAsync(string tag);
    }
}