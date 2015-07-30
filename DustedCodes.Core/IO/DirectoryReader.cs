using System.IO;

namespace DustedCodes.Core.IO
{
    public sealed class DirectoryReader : IDirectoryReader
    {
        public FileInfo[] GetFiles(string directoryPath, string searchPattern = null)
        {
            var directory = new DirectoryInfo(directoryPath);

            return string.IsNullOrEmpty(searchPattern)
                ? directory.GetFiles()
                : directory.GetFiles(searchPattern);
        }
    }
}