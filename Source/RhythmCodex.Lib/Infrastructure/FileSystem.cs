using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace RhythmCodex.Infrastructure
{
    [Service]
    public class FileSystem : IFileSystem
    {
        private const char SafeChar = '_';

        private readonly char[] InvalidChars = Path
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
        public IEnumerable<string> GetFileNames(string path) => Directory.GetFiles(path).Select(Path.GetFileName);

        /// <inheritdoc />
        public IEnumerable<string> GetDirectoryNames(string path) => Directory.GetDirectories(path).Select(Path.GetFileName);

        /// <inheritdoc />
        public string GetDirectory(string path)
        {
            var unrootedPath = Path.IsPathRooted(path)
                ? path.Substring(Path.GetPathRoot(path).Length)
                : path;

            return Path.GetDirectoryName(unrootedPath);
        }

        /// <inheritdoc />
        public string GetSafeFileName(string fileName) => 
            new string(fileName.Select(c => InvalidChars.Contains(c) ? SafeChar : c).ToArray());
    }
}