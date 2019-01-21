using System.Drawing;
using System.Linq;
using RhythmCodex.Cli.Helpers;
using RhythmCodex.Cli.Orchestration.Infrastructure;
using RhythmCodex.Gdi.Converters;
using RhythmCodex.Gdi.Streamers;
using RhythmCodex.Infrastructure;
using RhythmCodex.Infrastructure.Models;
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
        private readonly IGdiDsp _gdiDsp;

        public GraphicsTaskBuilder(
            IFileSystem fileSystem, 
            ILogger logger,
            IPngStreamWriter pngStreamWriter,
            ITgaStreamReader tgaStreamReader,
            ITgaDecoder tgaDecoder,
            IGdiDsp gdiDsp) 
            : base(fileSystem, logger)
        {
            _pngStreamWriter = pngStreamWriter;
            _tgaStreamReader = tgaStreamReader;
            _tgaDecoder = tgaDecoder;
            _gdiDsp = gdiDsp;
        }

        private RawBitmap CropImage(RawBitmap bitmap)
        {
            if (Args.Options.ContainsKey("+crop_ddr"))
            {
                if (bitmap.Width == 512 && bitmap.Height == 256)
                    bitmap = _gdiDsp.Snip(bitmap, new Rectangle(0, 0, 320, 200));
                else if (bitmap.Width == 256 && bitmap.Height == 128)
                    bitmap = _gdiDsp.Snip(bitmap, new Rectangle(0, 0, 256, 80));
                else if (bitmap.Width == 1024 && bitmap.Height == 512)
                    bitmap = _gdiDsp.Snip(bitmap, new Rectangle(0, 0, 640, 480));
            }

            return bitmap;
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
                        var bitmap = CropImage(_tgaDecoder.Decode(image));

                        using (var outStream = OpenWriteSingle(task, file, i => $"{i}.png"))
                            _pngStreamWriter.Write(outStream, bitmap);
                    }
                });

                return true;
            });
        }
    }
}