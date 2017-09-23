using System.Collections.Generic;
using System.IO;
using RhythmCodex.Infrastructure;

namespace RhythmCodex
{
    /// <summary>
    /// A file system that doesn't actually write to disk.
    /// </summary>
    public class FakeFileSystem : IFileSystem
    {
        private readonly IFileSystem _fileSystem;

        public FakeFileSystem(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }
        
        private readonly IDictionary<string, MemoryStream> _files = new Dictionary<string, MemoryStream>();

        /// <inheritdoc />
        public string GetFileName(string path) => _fileSystem.GetFileName(path);

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
        public string CombinePath(params string[] paths) => _fileSystem.CombinePath(paths);

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
            yield return path;
        }

        /// <inheritdoc />
        public IEnumerable<string> GetDirectoryNames(string path)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public string GetDirectory(string path) => _fileSystem.GetDirectory(path);

        /// <inheritdoc />
        public string GetSafeFileName(string fileName) => _fileSystem.GetSafeFileName(fileName);
    }
}