using System.IO;

namespace RhythmCodex.Infrastructure
{
    public class FileAccessor : IFileAccessor
    {
        private readonly string _basePath;

        public FileAccessor(string basePath)
        {
            _basePath = basePath;
        }

        public bool FileExists(string name) => 
            File.Exists(Path.Combine(_basePath, name));

        public Stream OpenRead(string name) => 
            File.OpenRead(Path.Combine(_basePath, name));
    }
}