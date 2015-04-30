using System.Collections.Generic;
using System.Threading.Tasks;

namespace DustedCodes.Blog.Data
{
    public interface IArticleReader
    {
        Task<Article> ReadAsync(string articleId);

        Task<IEnumerable<Article>> ReadAllAsync();
    }
}