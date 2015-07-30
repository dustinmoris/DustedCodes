using System.Threading.Tasks;

namespace DustedCodes.Blog.IO
{
    public interface IFileReader
    {
        Task<string> ReadAllTextAsync(string filePath);
    }
}