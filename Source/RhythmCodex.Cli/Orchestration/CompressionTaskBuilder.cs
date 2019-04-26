using System.IO;
using System.Linq;
using RhythmCodex.Cli.Helpers;
using RhythmCodex.Cli.Orchestration.Infrastructure;
using RhythmCodex.Compression;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;

namespace RhythmCodex.Cli.Orchestration
{
    [Service(singleInstance: false)]
    public class CompressionTaskBuilder : TaskBuilderBase<CompressionTaskBuilder>
    {
        private readonly IBemaniLzDecoder _bemaniLzDecoder;
        private readonly IBemaniLzEncoder _bemaniLzEncoder;

        public CompressionTaskBuilder(
            IFileSystem fileSystem,
            ILogger logger,
            IBemaniLzDecoder bemaniLzDecoder,
            IBemaniLzEncoder bemaniLzEncoder)
            : base(fileSystem, logger)
        {
            _bemaniLzDecoder = bemaniLzDecoder;
            _bemaniLzEncoder = bemaniLzEncoder;
        }

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
                    using (var stream = OpenRead(task, file))
                    {
                        var reader = new BinaryReader(stream);
                        var data = reader.ReadBytes((int) stream.Length);
                        var encoded = _bemaniLzEncoder.Encode(data);
                        task.Message = $"Compressed {data.Length} -> {encoded.Length} bytes.";
                            
                        using (var output = OpenWriteSingle(task, file, i => $"{i}.bemanilz"))
                        {
                            var writer = new BinaryWriter(output);
                            writer.Write(encoded);
                            output.Flush();
                        }
                    }
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
                    using (var stream = OpenRead(task, file))
                    {
                        var decoded = _bemaniLzDecoder.Decode(stream);
                        task.Message = $"Deompressed {stream.Length} -> {decoded.Length} bytes.";
                        using (var output = OpenWriteSingle(task, file, i => $"{i}.decoded"))
                        {
                            var writer = new BinaryWriter(output);
                            writer.Write(decoded);
                            output.Flush();
                        }
                    }
                });

                return true;
            });
        }
    }
}