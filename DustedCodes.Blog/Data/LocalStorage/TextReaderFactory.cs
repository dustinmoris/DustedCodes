using System.IO;

namespace DustedCodes.Blog.Data.LocalStorage
{
    public class TextReaderFactory : ITextReaderFactory
    {
        public TextReader FromFile(FileInfo fileInfo)
        {
            return new StreamReader(fileInfo.FullName);
        }
    }
}