using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace RhythmCodex.Infrastructure
{
    [Service]
    public class FileSystem : IFileSystem
    {
        private readonly ILogger _logger;

        public FileSystem(ILogger logger)
        {
            _logger = logger;
        }
        
        private const char SafeChar = '_';

        private readonly char[] _invalidChars = Path
            .GetInvalidFileNameChars()
            .Concat(Path.GetInvalidPathChars())
            .Distinct()
            .ToArray();

        /// <inheritdoc />
        public string GetFileName(string path)
        {
            return Path.GetFileName(path);
        }

        /// <inheritdoc />
        public Stream OpenRead(string path)
        {
            _logger.Debug($"Open for read: {path}");
            return File.OpenRead(path);
        }

        /// <inheritdoc />
        public Stream OpenWrite(string path)
        {
            _logger.Debug($"Open for write: {path}");
            return File.OpenWrite(path);
        }

        /// <inheritdoc />
        public string CombinePath(params string[] paths)
        {
            return Path.Combine(paths);
        }

        /// <inheritdoc />
        public string CurrentPath => Directory.GetCurrentDirectory();

        /// <inheritdoc />
        public byte[] ReadAllBytes(string path)
        {
            _logger.Debug($"Reading all bytes: {path}");
            return File.ReadAllBytes(path);
        }

        /// <inheritdoc />
        public void WriteAllBytes(string path, byte[] data)
        {
            _logger.Debug($"Writing all bytes: {path}");
            File.WriteAllBytes(path, data);
        }

        /// <inheritdoc />
        public void CreateDirectory(string path)
        {
            if (!Directory.Exists(path))
            {
                _logger.Debug($"Creating directory: {path}");
                Directory.CreateDirectory(path);                
            }
        }

        /// <inheritdoc />
        public IEnumerable<string> GetFileNames(string path, string pattern)
        {
            if (string.IsNullOrWhiteSpace(path))
                path = ".\\";

            _logger.Debug(string.IsNullOrWhiteSpace(pattern)
                ? $"Getting all files from path: {path} (no pattern)"
                : $"Getting all files from path: {path} (with pattern {pattern})");

            return string.IsNullOrWhiteSpace(pattern) 
                ? Directory.GetFiles(path) 
                : Directory.GetFiles(path, pattern);
        }

        /// <inheritdoc />
        public IEnumerable<string> GetDirectoryNames(string path)
        {
            _logger.Debug($"Getting all directories from path: {path}");
            return Directory.GetDirectories(path).Select(Path.GetFileName);
        }

        /// <inheritdoc />
        public string GetDirectory(string path)
        {
            return Path.GetDirectoryName(path);
        }

        /// <inheritdoc />
        public string GetSafeFileName(string fileName)
        {
            return new string(fileName.Select(c => _invalidChars.Contains(c) ? SafeChar : c).ToArray());
        }
    }
}