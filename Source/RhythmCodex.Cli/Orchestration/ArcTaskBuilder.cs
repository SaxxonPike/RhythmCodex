using System.Linq;
using RhythmCodex.Arc.Streamers;
using RhythmCodex.Cli.Helpers;
using RhythmCodex.Cli.Orchestration.Infrastructure;
using RhythmCodex.Compression;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;

namespace RhythmCodex.Cli.Orchestration
{
    [Service(singleInstance: false)]
    public class ArcTaskBuilder : TaskBuilderBase<ArcTaskBuilder>
    {
        private readonly IArcStreamReader _arcStreamReader;
        private readonly IArcLzDecoder _arcLzDecoder;

        public ArcTaskBuilder(IFileSystem fileSystem, ILogger logger,
            IArcStreamReader arcStreamReader,
            IArcLzDecoder arcLzDecoder)
            : base(fileSystem, logger)
        {
            _arcStreamReader = arcStreamReader;
            _arcLzDecoder = arcLzDecoder;
        }

        public ITask CreateExtract()
        {
            return Build("Extract ARC", task =>
            {
                var files = GetInputFiles(task);
                if (!files.Any())
                {
                    task.Message = "No input files.";
                    return false;
                }

                ParallelProgress(task, files, file =>
                {
                    using var stream = OpenRead(task, file);

                    var entries = _arcStreamReader.Read(stream).ToArray();
                    var index = 0;

                    foreach (var entry in entries)
                    {
                        task.Progress = index / (float) entries.Length;
                        task.Message = $"Extracting {entry.Name}";
                        using var outFile = OpenWriteSingle(task, file, _ => entry.Name);
                        outFile.Write(_arcLzDecoder.Decode(entry.Data));
                        outFile.Flush();
                    }
                });

                return true;
            });
        }
    }
}