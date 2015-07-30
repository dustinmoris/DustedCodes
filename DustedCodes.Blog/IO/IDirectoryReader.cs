using System.Collections.Generic;
using System.IO;

namespace DustedCodes.Blog.IO
{
    public interface IDirectoryReader
    {
        FileInfo[] GetFiles(string directoryPath, string searchPattern = null);
    }
}