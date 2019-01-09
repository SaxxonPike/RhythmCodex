using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using RhythmCodex.Cli.Helpers;

namespace RhythmCodex.Cli.Orchestration.Infrastructure
{
    public class TaskFileFactory : ITaskFileFactory
    {
        private readonly IFileSystem _fileSystem;
        private ISet<IDisposable> _disposables = new HashSet<IDisposable>();

        public TaskFileFactory(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }
        
        public TaskFile CreateFromFile(string path)
        {
            return new TaskFile
            {
                FileName = Path.GetFileName(path),
                Path = Path.GetDirectoryName(path),
                Open = () => _fileSystem.OpenRead(path)
            };
        }

        public IReadOnlyList<TaskFile> CreateFromArchive(string path)
        {
            var archive = new ZipArchive(_fileSystem.OpenRead(path), ZipArchiveMode.Read, false);
            throw new System.NotImplementedException();
        }
    }

    public interface ITaskFileFactory
    {
        TaskFile CreateFromFile(string path);
        IReadOnlyList<TaskFile> CreateFromArchive(string path);
    }
}