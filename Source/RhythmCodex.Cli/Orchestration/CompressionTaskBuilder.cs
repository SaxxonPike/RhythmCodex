using System.IO;
using System.Linq;
using ClientCommon;
using RhythmCodex.Cli.Orchestration.Infrastructure;
using RhythmCodex.Compression;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;

namespace RhythmCodex.Cli.Orchestration;

[Service(singleInstance: false)]
public class CompressionTaskBuilder(
    IFileSystem fileSystem,
    ILogger logger,
    IBemaniLzDecoder bemaniLzDecoder,
    IBemaniLzEncoder bemaniLzEncoder)
    : TaskBuilderBase<CompressionTaskBuilder>(fileSystem, logger)
{
    public ITask CreateCompressBemaniLz()
    {
        return Build("Compress Bemani LZ", task =>
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
                var reader = new BinaryReader(stream);
                var data = reader.ReadBytes((int) stream.Length);
                var encoded = bemaniLzEncoder.Encode(data);
                task.Message = $"Compressed {data.Length} -> {encoded.Length} bytes.";

                using var output = OpenWriteSingle(task, file, i => $"{i}.bemanilz");
                output.Write(encoded.Span);
                output.Flush();
            });

            return true;
        });
    }
        
    public ITask CreateDecompressBemaniLz()
    {
        return Build("Decompress Bemani LZ", task =>
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
                var decoded = bemaniLzDecoder.Decode(stream);
                task.Message = $"Deompressed {stream.Length} -> {decoded.Length} bytes.";
                using var output = OpenWriteSingle(task, file, i => $"{i}.decoded");
                output.Write(decoded.Span);
                output.Flush();
            });

            return true;
        });
    }
}