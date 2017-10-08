using System.Collections.Generic;
using RhythmCodex.Extensions;

namespace RhythmCodex.Infrastructure
{
    /// <inheritdoc />
    [Service]
    public class FileRouter : IFileRouter
    {
        private readonly IFileSystem _fileSystem;

        /// <summary>
        ///     Create a FileRouter which will interface with the specified IFileSystem.
        /// </summary>
        public FileRouter(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }

        /// <inheritdoc />
        public void Export(string path, IEnumerable<IRoutable> routables)
        {
            foreach (var routable in routables)
                switch (routable)
                {
                    case IFileRoute file:
                        _fileSystem.WriteAllBytes(_fileSystem.CombinePath(path, _fileSystem.GetSafeFileName(file.Name)),
                            file.Data);
                        break;
                    case IDirectoryRoute directory:
                        var subPath = _fileSystem.CombinePath(path, _fileSystem.GetSafeFileName(directory.Name));
                        _fileSystem.CreateDirectory(subPath);
                        Export(subPath, directory.Entries);
                        break;
                }
        }

        /// <inheritdoc />
        public IEnumerable<IRoutable> Import(string path)
        {
            foreach (var fileName in _fileSystem.GetFileNames(path))
                yield return new FileRoute
                {
                    Name = fileName,
                    Data = _fileSystem.ReadAllBytes(_fileSystem.CombinePath(path, fileName))
                };

            foreach (var dirName in _fileSystem.GetDirectoryNames(path))
                yield return new DirectoryRoute
                {
                    Name = dirName,
                    Entries = Import(_fileSystem.CombinePath(path, dirName))
                };
        }

        /// <inheritdoc />
        public IEnumerable<IFileRoute> Flatten(IEnumerable<IRoutable> routables)
        {
            foreach (var routable in routables)
                switch (routable)
                {
                    case IFileRoute file:
                        yield return file;
                        break;
                    case IDirectoryRoute directory:
                        foreach (var file in Flatten(directory.Entries))
                            yield return new FileRoute
                            {
                                Data = file.Data,
                                Name = _fileSystem.CombinePath(directory.Name, file.Name)
                            };
                        break;
                }
        }
    }
}