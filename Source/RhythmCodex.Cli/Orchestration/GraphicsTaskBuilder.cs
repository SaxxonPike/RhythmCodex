using System.Linq;
using RhythmCodex.Cli.Helpers;
using RhythmCodex.Cli.Orchestration.Infrastructure;
using RhythmCodex.Gdi.Streamers;
using RhythmCodex.Infrastructure;
using RhythmCodex.Tga.Converters;
using RhythmCodex.Tga.Streamers;

namespace RhythmCodex.Cli.Orchestration
{
    [InstancePerDependency]
    public class GraphicsTaskBuilder : TaskBuilderBase<GraphicsTaskBuilder>
    {
        private readonly IPngStreamWriter _pngStreamWriter;
        private readonly ITgaStreamReader _tgaStreamReader;
        private readonly ITgaDecoder _tgaDecoder;

        public GraphicsTaskBuilder(
            IFileSystem fileSystem, 
            ILogger logger,
            IPngStreamWriter pngStreamWriter,
            ITgaStreamReader tgaStreamReader,
            ITgaDecoder tgaDecoder) 
            : base(fileSystem, logger)
        {
            _pngStreamWriter = pngStreamWriter;
            _tgaStreamReader = tgaStreamReader;
            _tgaDecoder = tgaDecoder;
        }

        public ITask CreateDecodeTga()
        {
            return Build("Decode TGA", task =>
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
                        var image = _tgaStreamReader.Read(stream, (int) stream.Length);
                        task.Message = "Decoding TGA.";
                        var bitmap = _tgaDecoder.Decode(image);
                        using (var outStream = OpenWriteSingle(task, file, i => $"{i}.png"))
                            _pngStreamWriter.Write(outStream, bitmap);
                    }
                });

                return true;
            });
        }
    }
}