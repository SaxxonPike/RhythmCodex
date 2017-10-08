using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using RhythmCodex.Infrastructure;

namespace RhythmCodex
{
    /// <summary>
    ///     A file system that doesn't actually write to disk.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class FakeFileSystem : IFileSystem
    {
        private readonly IDictionary<string, MemoryStream> _files = new Dictionary<string, MemoryStream>();
        private readonly IFileSystem _fileSystem;

        public FakeFileSystem(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }

        /// <inheritdoc />
        public string GetFileName(string path)
        {
            return _fileSystem.GetFileName(path);
        }

        /// <inheritdoc />
        public Stream OpenRead(string path)
        {
            if (!_files.ContainsKey(path))
                throw new IOException($"File not found: {path}");

            var result = new MemoryStream(_files[path].ToArray());
            return result;
        }

        /// <inheritdoc />
        public Stream OpenWrite(string path)
        {
            if (_files.ContainsKey(path))
                _files[path].Dispose();

            var result = new MemoryStream();
            _files[path] = result;
            return result;
        }

        /// <inheritdoc />
        public string CombinePath(params string[] paths)
        {
            return _fileSystem.CombinePath(paths);
        }

        /// <inheritdoc />
        public string CurrentPath => new string(Path.DirectorySeparatorChar, 1);

        /// <inheritdoc />
        public byte[] ReadAllBytes(string path)
        {
            if (!_files.ContainsKey(path))
                throw new IOException($"File not found: {path}");

            return _files[path].ToArray();
        }

        /// <inheritdoc />
        public void WriteAllBytes(string path, byte[] data)
        {
            _files[path] = new MemoryStream(data);
        }

        /// <inheritdoc />
        public void CreateDirectory(string path)
        {
        }

        /// <inheritdoc />
        public IEnumerable<string> GetFileNames(string path, string pattern)
        {
            return _files.Where(f => f.Key.StartsWith(path, StringComparison.OrdinalIgnoreCase)).Select(f => f.Key);
        }

        /// <inheritdoc />
        public IEnumerable<string> GetDirectoryNames(string path)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public string GetDirectory(string path)
        {
            return _fileSystem.GetDirectory(path);
        }

        /// <inheritdoc />
        public string GetSafeFileName(string fileName)
        {
            return _fileSystem.GetSafeFileName(fileName);
        }
    }
}