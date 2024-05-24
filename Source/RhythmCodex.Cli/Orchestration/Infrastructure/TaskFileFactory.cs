using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using ClientCommon;

namespace RhythmCodex.Cli.Orchestration.Infrastructure;

public class TaskFileFactory(IFileSystem fileSystem) : ITaskFileFactory
{
    private ISet<IDisposable> _disposables = new HashSet<IDisposable>();

    public TaskFile CreateFromFile(string path)
    {
        return new TaskFile
        {
            FileName = Path.GetFileName(path),
            Path = Path.GetDirectoryName(path),
            Open = () => fileSystem.OpenRead(path)
        };
    }

    public IReadOnlyList<TaskFile> CreateFromArchive(string path)
    {
        var archive = new ZipArchive(fileSystem.OpenRead(path), ZipArchiveMode.Read, false);
        throw new System.NotImplementedException();
    }
}