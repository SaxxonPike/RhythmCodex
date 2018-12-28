using System.Collections.Generic;

namespace RhythmCodex.Cli.Helpers
{
    public static class FileSystemExtensions
    {
        public static IEnumerable<string> GetFileNames(this IFileSystem fileSystem, string path, bool recursive = false)
        {
            var directory = fileSystem.GetDirectory(path);
            var pattern = fileSystem.GetFileName(path);
            return fileSystem.GetFileNames(directory, pattern, recursive);
        }
    }
}