using System.Threading.Tasks;

namespace DustedCodes.Blog.Data
{
    public interface IArticleParser
    {
        Task<Article> ParseAsync(string id, string rawContent);
    }
}