using System.IO;
using System.Threading.Tasks;

namespace DustedCodes.Blog.Data.LocalStorage
{
    public interface IArticleParser
    {
        Task<Article> ParseAsync(FileInfo fileInfo);
    }
}