using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using ClientCommon;
using RhythmCodex.Infrastructure;

namespace RhythmCodex.Cli.Orchestration.Infrastructure;

public abstract class TaskBuilderBase<TTask>(
    IFileSystem fileSystem,
    ILogger logger)
    where TTask : TaskBuilderBase<TTask>
{
    protected Args Args { get; private set; }
    private ILogger Logger { get; } = logger;

    protected void ParallelProgress<T>(BuiltTask task, ICollection<T> items, Action<T> action)
    {
        var progressIncrement = 1 / (float) items.Count;
        foreach (var item in items.AsParallel())
        {
            action(item);
            task.Progress += progressIncrement;
        }
    }

    protected ITask Build(string name, Func<BuiltTask?, bool> task)
    {
        BuiltTask? result = null;
        result = new BuiltTask(name, RunTask);
        result.MessageUpdated += (_, message) => Logger.Info(message);
        return result;

        bool RunTask() => task(result);
    }

    protected Stream OpenRelatedRead(InputFile inputFile, Func<string, string> transformName)
    {
        var transformedName = transformName(Path.GetFileNameWithoutExtension(inputFile.Name));
        return inputFile.OpenRelated(transformedName);
    }

    protected string? GetOutputFolder(string inputFile) =>
        Args.OutputPath ?? Path.GetDirectoryName(inputFile);

    protected InputFile[] GetInputFiles(BuiltTask task)
    {
        task.Message = "Resolving input files.";
        var sourceFiles = Args.InputFiles
            .SelectMany(a => fileSystem.GetFileNames(a, Args.RecursiveInputFiles));

        if (Args.FilesAreZipArchives)
            return GetArchivedFiles(sourceFiles);

        return sourceFiles
            .Select(sf =>
            {
                var info = new FileInfo(sf);

                if (info.Exists)
                {
                    return new InputFile(
                            sf,
                            () => fileSystem.OpenRead(sf),
                            f => fileSystem.OpenRead(Path.Combine(Path.GetDirectoryName(sf), f)),
                            s => s.Dispose())
                        { Length = info.Length };
                }
                else
                {
                    throw new RhythmCodexException($"File doesn't exist: {sf}");
                }
            })
            .ToArray();
    }

    private InputFile[] GetArchivedFiles(IEnumerable<string> sourceFiles)
    {
        return sourceFiles
            .SelectMany(sf =>
            {
                using var parentArc = new ZipArchive(fileSystem.OpenRead(sf));
                return parentArc.Entries.Select(e => new InputFile(
                    Path.Combine(Path.GetDirectoryName(sf), Path.GetFileNameWithoutExtension(sf), e.FullName),
                    () =>
                    {
                        var archive = new ZipArchive(fileSystem.OpenRead(sf));
                        var entry = archive.GetEntry(e.Name);
                        return entry.Open();
                    },
                    f =>
                    {
                        var archive = new ZipArchive(fileSystem.OpenRead(sf));
                        var entry = archive.GetEntry(Path.Combine(Path.GetDirectoryName(sf), f));
                        return entry.Open();
                    },
                    s =>
                    {
                        s.Dispose();
                        parentArc.Dispose();
                    }) {Length = e.Length} );
            }).ToArray();
    }

    protected InputFile GetInputFileDirect(string name)
    {
        var sourceFiles = fileSystem.GetFileNames(name, Args.RecursiveInputFiles);

        if (Args.FilesAreZipArchives)
            return GetArchivedFiles(sourceFiles).Single();

        return sourceFiles
            .Select(sf =>
            {
                var info = new FileInfo(sf);
                if (info.Exists)
                {
                    return new InputFile(
                        sf,
                        () => fileSystem.OpenRead(sf),
                        f => fileSystem.OpenRead(Path.Combine(Path.GetDirectoryName(sf), f)),
                        s => s.Dispose())
                    {
                        Length = info.Length
                    };
                }
                else
                {
                    throw new RhythmCodexException($"File doesn't exist: {sf}");
                }
            })
            .Single();
    }

    protected byte[] GetFile(BuiltTask task, InputFile inputFile)
    {
        task.Message = $"Reading {inputFile}";
        using var stream = inputFile.Open();
        var reader = new BinaryReader(stream);
        return reader.ReadBytes((int) stream.Length);
    }

    protected Stream OpenRead(BuiltTask task, InputFile inputFile)
    {
        task.Message = $"Opening {inputFile.Name}";
        return inputFile.Open();
    }

    protected Stream OpenWriteSingle(BuiltTask task, InputFile inputFile, Func<string?, string?> generateName)
    {
        if (fileSystem == null)
            throw new RhythmCodexException("Filesystem is not defined.");

        var path = GetOutputFolder(inputFile.Name);
        var newName = generateName(Path.GetFileNameWithoutExtension(inputFile.Name));
        var newPath = Path.Combine(path, newName);
        var newDirectory = Path.GetDirectoryName(newPath);
        task.Message = $"Writing {newPath}";
        fileSystem.CreateDirectory(newDirectory);
        return fileSystem.OpenWrite(newPath);
    }

    protected Stream OpenWriteMulti(BuiltTask task, InputFile inputFile, Func<string, string> generateName)
    {
        if (fileSystem == null)
            throw new RhythmCodexException("Filesystem is not defined.");
        var baseName = Path.GetFileNameWithoutExtension(inputFile.Name);
        var path = Path.Combine(GetOutputFolder(inputFile.Name), baseName);
        var newName = generateName(baseName);
        var newPath = Path.Combine(path, newName);
        var newDirectory = Path.GetDirectoryName(newPath);
        task.Message = $"Writing {newPath}";
        fileSystem.CreateDirectory(newDirectory);
        return fileSystem.OpenWrite(newPath);
    }

    public TTask WithArgs(Args args)
    {
        this.Args = args;
        return (TTask) this;
    }

    protected sealed class BuiltTask(string name, Func<bool> run) : ITask
    {
        public event EventHandler<string> MessageUpdated;
        public event EventHandler<float> ProgressUpdated;
        private string _message = string.Empty;
        private float _progress = 0;

        public string Id { get; } = Guid.NewGuid().ToString();
        public string Name { get; } = name;

        public float Progress
        {
            get => _progress;
            set
            {
                _progress = value;
                ProgressUpdated?.Invoke(this, value);
            }
        }

        public Func<bool> Run => run;

        public string Message
        {
            get => _message;
            set
            {
                _message = value;
                MessageUpdated?.Invoke(this, value);
            }
        }
    }
}