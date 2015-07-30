using System.IO;

namespace DustedCodes.Core.IO
{
    public interface IDirectoryReader
    {
        FileInfo[] GetFiles(string directoryPath, string searchPattern = null);
    }
}