using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using RhythmCodex.Cli.Helpers;
using RhythmCodex.Infrastructure;

namespace RhythmCodex.Cli.Orchestration.Infrastructure
{
    public abstract class TaskBuilderBase<TTask> where TTask : TaskBuilderBase<TTask>
    {
        private readonly IFileSystem _fileSystem;

        protected TaskBuilderBase(
            IFileSystem fileSystem,
            ILogger logger)
        {
            _fileSystem = fileSystem;
            Logger = logger;
        }
        
        protected Args Args { get; private set; }
        private ILogger Logger { get; }

        protected void ParallelProgress<T>(BuiltTask task, ICollection<T> items, Action<T> action)
        {
            var progressIncrement = 1 / (float) items.Count;
            foreach (var item in items.AsParallel())
            {
                action(item);
                task.Progress += progressIncrement;
            }
        }

        protected ITask Build(string name, Func<BuiltTask, bool> task)
        {
            BuiltTask result = null;
            result = new BuiltTask(name, () => task(result));
            result.MessageUpdated += (sender, message) => Logger.Info(message);
            return result;
        }

        protected string GetOutputFolder(string inputFile) => 
            Args.OutputPath ?? Path.GetDirectoryName(inputFile);

        protected string[] GetInputFiles(BuiltTask task)
        {
            task.Message = "Resolving input files.";
            return Args.InputFiles
                .SelectMany(a => _fileSystem.GetFileNames(a, Args.RecursiveInputFiles))
                .ToArray();
        }

        protected byte[] GetFile(BuiltTask task, string inputFile)
        {
            task.Message = $"Reading {inputFile}";
            return _fileSystem.ReadAllBytes(inputFile);
        }

        protected Stream OpenRead(BuiltTask task, string inputFile)
        {
            task.Message = $"Opening {inputFile}";
            return _fileSystem.OpenRead(inputFile);
        }

        protected Stream OpenWriteSingle(BuiltTask task, string inputFile, Func<string, string> generateName)
        {
            if (_fileSystem == null)
                throw new RhythmCodexException("Filesystem is not defined.");

            var path = GetOutputFolder(inputFile);
            var newName = generateName(Path.GetFileNameWithoutExtension(inputFile));
            var newPath = Path.Combine(path, newName);
            var newDirectory = Path.GetDirectoryName(newPath);
            task.Message = $"Writing {newPath}";
            _fileSystem.CreateDirectory(newDirectory);
            return _fileSystem.OpenWrite(newPath);
        }

        protected Stream OpenWriteMulti(BuiltTask task, string inputFile, Func<string, string> generateName)
        {
            if (_fileSystem == null)
                throw new RhythmCodexException("Filesystem is not defined.");

            var baseName = Path.GetFileNameWithoutExtension(inputFile);
            var path = Path.Combine(GetOutputFolder(inputFile), baseName);
            var newName = generateName(baseName);
            var newPath = Path.Combine(path, newName);
            var newDirectory = Path.GetDirectoryName(newPath);
            task.Message = $"Writing {newPath}";
            _fileSystem.CreateDirectory(newDirectory);
            return _fileSystem.OpenWrite(newPath);
        }

        public TTask WithArgs(Args args)
        {
            this.Args = args;
            return (TTask) this;
        }

        protected sealed class BuiltTask : ITask
        {
            public event EventHandler<string> MessageUpdated;
            public event EventHandler<float> ProgressUpdated;
            
            private string _message = string.Empty;
            private float _progress = 0;

            public BuiltTask(string name, Func<bool> run)
            {
                Name = name;
                Run = run;
            }
            
            public string Name { get; }

            public float Progress
            {
                get => _progress;
                set
                {
                    _progress = value;
                    ProgressUpdated?.Invoke(this, value);
                }
            }

            public Func<bool> Run { get; set; }

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
}