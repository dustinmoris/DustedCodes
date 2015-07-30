using System.IO;

namespace DustedCodes.Core.IO
{
    public class TextReaderFactory : ITextReaderFactory
    {
        public TextReader FromFile(FileInfo fileInfo)
        {
            return new StreamReader(fileInfo.FullName);
        }
    }
}