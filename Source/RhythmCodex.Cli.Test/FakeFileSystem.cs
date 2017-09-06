using System.Collections.Generic;
using System.IO;
using RhythmCodex.Infrastructure;

namespace RhythmCodex.Cli
{
    public class FakeFileSystem : IFileSystem
    {
        private readonly IDictionary<string, MemoryStream> _files = new Dictionary<string, MemoryStream>();
        
        public string GetFileName(string path) => Path.GetFileName(path);

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

        public string CombinePath(params string[] paths) => Path.Combine(paths);

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
    }
}