using System.Linq;
using ClientCommon;
using RhythmCodex.Arc.Streamers;
using RhythmCodex.Cli.Orchestration.Infrastructure;
using RhythmCodex.Compression;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;

namespace RhythmCodex.Cli.Orchestration;

[Service(singleInstance: false)]
public class ArcTaskBuilder(
    IFileSystem fileSystem,
    ILogger logger,
    IArcStreamReader arcStreamReader,
    IArcLzDecoder arcLzDecoder)
    : TaskBuilderBase<ArcTaskBuilder>(fileSystem, logger)
{
    public ITask CreateExtract()
    {
        return Build("Extract ARC", task =>
        {
            var files = GetInputFiles(task);
            if (files.Length == 0)
            {
                task.Message = "No input files.";
                return false;
            }

            ParallelProgress(task, files, file =>
            {
                using var stream = OpenRead(task, file);

                var entries = arcStreamReader.Read(stream).ToArray();
                var index = 0;

                foreach (var entry in entries)
                {
                    task.Progress = index / (float) entries.Length;
                    task.Message = $"Extracting {entry.Name}";
                    using var outFile = OpenWriteSingle(task, file, _ => entry.Name);
                    using var str = new ReadOnlyMemoryStream(entry.Data);
                    outFile.Write(arcLzDecoder.Decode(str).Span);
                    outFile.Flush();
                }
            });

            return true;
        });
    }
}