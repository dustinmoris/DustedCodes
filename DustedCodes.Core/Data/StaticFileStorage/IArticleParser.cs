using System.IO;
using System.Threading.Tasks;

namespace DustedCodes.Core.Data.StaticFileStorage
{
    public interface IArticleParser
    {
        Task<Article> ParseAsync(FileInfo fileInfo);
    }
}