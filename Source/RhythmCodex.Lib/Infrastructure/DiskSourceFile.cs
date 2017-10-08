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

        /// <inheritdoc />
        public void Dispose()
        {
        }

        /// <inheritdoc />
        public Stream OpenRead() => _fileSystem.OpenRead(_path);

        /// <inheritdoc />
        public string Name => _fileSystem.GetFileName(_path);
    }
}