using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace RhythmCodex.Infrastructure
{
    [Service]
    public class FileSystem : IFileSystem
    {
        private const char SafeChar = '_';

        private readonly char[] _invalidChars = Path
            .GetInvalidFileNameChars()
            .Concat(Path.GetInvalidPathChars())
            .Distinct()
            .ToArray();

        /// <inheritdoc />
        public string GetFileName(string path) => Path.GetFileName(path);

        /// <inheritdoc />
        public Stream OpenRead(string path) => File.OpenRead(path);

        /// <inheritdoc />
        public Stream OpenWrite(string path) => File.OpenWrite(path);

        /// <inheritdoc />
        public string CombinePath(params string[] paths) => Path.Combine(paths);

        /// <inheritdoc />
        public string CurrentPath => Directory.GetCurrentDirectory();

        /// <inheritdoc />
        public byte[] ReadAllBytes(string path) => File.ReadAllBytes(path);

        /// <inheritdoc />
        public void WriteAllBytes(string path, byte[] data) => File.WriteAllBytes(path, data);

        /// <inheritdoc />
        public void CreateDirectory(string path)
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
        }

        /// <inheritdoc />
        public IEnumerable<string> GetFileNames(string path, string pattern)
        {
            if (string.IsNullOrWhiteSpace(path))
                path = ".\\";
            return Directory.GetFiles(path, pattern);            
        }

        /// <inheritdoc />
        public IEnumerable<string> GetDirectoryNames(string path) => Directory.GetDirectories(path).Select(Path.GetFileName);

        /// <inheritdoc />
        public string GetDirectory(string path)
        {
//            var rootedPath = Path.IsPathRooted(path)
//                ? path
//                : Path.Combine(Path.GetPathRoot(CurrentPath), path);

            return Path.GetDirectoryName(path);
        }

        /// <inheritdoc />
        public string GetSafeFileName(string fileName) => 
            new string(fileName.Select(c => _invalidChars.Contains(c) ? SafeChar : c).ToArray());
    }
}