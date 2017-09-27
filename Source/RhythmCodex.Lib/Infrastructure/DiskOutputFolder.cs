using System.IO;

namespace RhythmCodex.Infrastructure
{
    public class DiskOutputFolder : IOutputFolder
    {
        private readonly IFileSystem _fileSystem;
        private readonly string _path;

        public DiskOutputFolder(IFileSystem fileSystem, string path)
        {
            _fileSystem = fileSystem;
            _path = path;
        }

        /// <inheritdoc />
        public void Dispose()
        {
        }

        /// <inheritdoc />
        public string GetPath(params string[] subpaths) => _fileSystem.CombinePath(subpaths);

        /// <inheritdoc />
        public Stream OpenWrite(string path) => _fileSystem.OpenWrite(_fileSystem.CombinePath(_path, path));
    }
}