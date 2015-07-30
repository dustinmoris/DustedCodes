using System.Collections.Generic;
using System.Threading.Tasks;

namespace DustedCodes.Blog.Data
{
    public interface IArticleRepository
    {
        Task<Article> GetAsync(string id);

        Task<ICollection<Article>> GetAllOrderedByDateAsync();
    }
}