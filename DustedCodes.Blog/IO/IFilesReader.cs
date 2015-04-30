using System.Threading.Tasks;

namespace DustedCodes.Blog.IO
{
    public interface IFilesReader
    {
        Task<string> ReadAllTextAsync(string filePath);
    }
}