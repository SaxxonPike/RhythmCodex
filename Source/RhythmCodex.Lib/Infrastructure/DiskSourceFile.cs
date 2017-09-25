using System.IO;

namespace RhythmCodex.Infrastructure
{
    public class DiskSourceFile : ISourceFile
    {
        private readonly IFileSystem _fileSystem;
        private readonly string _path;

        public DiskSourceFile(IFileSystem fileSystem, string path)
        {
            _fileSystem = fileSystem;
            _path = path;
        }

        public void Dispose()
        {
        }

        public Stream OpenRead() => _fileSystem.OpenRead(_path);

        public string Name => _fileSystem.GetFileName(_path);
    }
}