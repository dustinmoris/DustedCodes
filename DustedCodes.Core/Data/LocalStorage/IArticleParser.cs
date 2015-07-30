using System.IO;
using System.Threading.Tasks;

namespace DustedCodes.Core.Data.LocalStorage
{
    public interface IArticleParser
    {
        Task<Article> ParseAsync(FileInfo fileInfo);
    }
}