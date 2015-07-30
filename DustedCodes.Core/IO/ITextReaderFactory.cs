using System.IO;

namespace DustedCodes.Core.IO
{
    public interface ITextReaderFactory
    {
        TextReader FromFile(FileInfo fileInfo);
    }
}