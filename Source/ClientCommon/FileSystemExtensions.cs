using System.Collections.Generic;
using System.Linq;

namespace ClientCommon
{
    public static class FileSystemExtensions
    {
        public static IEnumerable<string> GetFileNames(this IFileSystem fileSystem, string path, bool recursive = false)
        {
            var directory = fileSystem.GetDirectory(path);
            var pattern = fileSystem.GetFileName(path);
            return fileSystem.GetFileNames(directory, pattern, recursive);
        }

        public static IEnumerable<string> GetFileNames(this IFileSystem fileSystem, IEnumerable<string> paths,
            bool recursive = false) =>
            paths.SelectMany(p => GetFileNames(fileSystem, p, recursive)).Distinct();
    }
}