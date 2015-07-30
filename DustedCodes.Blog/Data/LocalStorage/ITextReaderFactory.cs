using System.IO;

namespace DustedCodes.Blog.Data.LocalStorage
{
    public interface ITextReaderFactory
    {
        TextReader FromFile(FileInfo fileInfo);
    }
}