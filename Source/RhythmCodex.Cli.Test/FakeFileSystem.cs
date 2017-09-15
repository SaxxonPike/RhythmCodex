using System.Collections.Generic;
using System.IO;
using RhythmCodex.Infrastructure;

namespace RhythmCodex.Cli
{
    public class FakeFileSystem : IFileSystem
    {
        private readonly IFileSystem _fileSystem;

        public FakeFileSystem(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }
        
        private readonly IDictionary<string, MemoryStream> _files = new Dictionary<string, MemoryStream>();
        
        public string GetFileName(string path) => _fileSystem.GetFileName(path);

        public Stream OpenRead(string path)
        {
            if (!_files.ContainsKey(path)) 
                throw new IOException($"File not found: {path}");
            
            var result = new MemoryStream(_files[path].ToArray());
            return result;
        }

        public Stream OpenWrite(string path)
        {
            if (_files.ContainsKey(path))
                _files[path].Dispose();
            
            var result = new MemoryStream();
            _files[path] = result;
            return result;
        }

        public string CombinePath(params string[] paths) => _fileSystem.CombinePath(paths);

        public string CurrentPath => new string(Path.DirectorySeparatorChar, 1);
        
        public byte[] ReadAllBytes(string path)
        {
            if (!_files.ContainsKey(path)) 
                throw new IOException($"File not found: {path}");

            return _files[path].ToArray();
        }

        public void WriteAllBytes(string path, byte[] data)
        {
            _files[path] = new MemoryStream(data);
        }

        public void CreateDirectory(string path)
        {
        }

        public IEnumerable<string> GetFileNames(string path)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<string> GetDirectoryNames(string path)
        {
            throw new System.NotImplementedException();
        }

        public string GetDirectory(string path) => _fileSystem.GetDirectory(path);

        public string GetSafeFileName(string fileName) => _fileSystem.GetSafeFileName(fileName);
    }
}