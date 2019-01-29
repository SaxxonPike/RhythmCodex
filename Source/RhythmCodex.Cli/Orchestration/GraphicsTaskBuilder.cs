using System.Drawing;
using System.Linq;
using RhythmCodex.Cli.Helpers;
using RhythmCodex.Cli.Orchestration.Infrastructure;
using RhythmCodex.Dds.Converters;
using RhythmCodex.Dds.Streamers;
using RhythmCodex.Dsp;
using RhythmCodex.Gdi.Converters;
using RhythmCodex.Gdi.Streamers;
using RhythmCodex.Infrastructure;
using RhythmCodex.Infrastructure.Models;
using RhythmCodex.IoC;
using RhythmCodex.Tga.Converters;
using RhythmCodex.Tga.Streamers;

namespace RhythmCodex.Cli.Orchestration
{
    [Service(singleInstance: false)]
    public class GraphicsTaskBuilder : TaskBuilderBase<GraphicsTaskBuilder>
    {
        private readonly IPngStreamWriter _pngStreamWriter;
        private readonly ITgaStreamReader _tgaStreamReader;
        private readonly ITgaDecoder _tgaDecoder;
        private readonly IGraphicDsp _graphicDsp;
        private readonly IDdsStreamReader _ddsStreamReader;
        private readonly IDdsBitmapDecoder _ddsBitmapDecoder;

        public GraphicsTaskBuilder(
            IFileSystem fileSystem, 
            ILogger logger,
            IPngStreamWriter pngStreamWriter,
            ITgaStreamReader tgaStreamReader,
            ITgaDecoder tgaDecoder,
            IGraphicDsp graphicDsp,
            IDdsStreamReader ddsStreamReader,
            IDdsBitmapDecoder ddsBitmapDecoder
            ) 
            : base(fileSystem, logger)
        {
            _pngStreamWriter = pngStreamWriter;
            _tgaStreamReader = tgaStreamReader;
            _tgaDecoder = tgaDecoder;
            _graphicDsp = graphicDsp;
            _ddsStreamReader = ddsStreamReader;
            _ddsBitmapDecoder = ddsBitmapDecoder;
        }

        private RawBitmap CropImage(RawBitmap bitmap)
        {
            if (Args.Options.ContainsKey("+crop_ddr"))
            {
                if (bitmap.Width == 512 && bitmap.Height == 256)
                    bitmap = _graphicDsp.Snip(bitmap, new Rectangle(0, 0, 320, 200));
                else if (bitmap.Width == 256 && bitmap.Height == 128)
                    bitmap = _graphicDsp.Snip(bitmap, new Rectangle(0, 0, 256, 80));
                else if (bitmap.Width == 1024 && bitmap.Height == 512)
                    bitmap = _graphicDsp.Snip(bitmap, new Rectangle(0, 0, 640, 480));
            }

            return bitmap;
        }

        public ITask CreateDecodeDds()
        {
            return Build("Decode DirectDraw Surface", task =>
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
                        var image = _ddsStreamReader.Read(stream, (int) stream.Length);
                        task.Message = "Decoding DDS.";
                        var bitmap = CropImage(_ddsBitmapDecoder.Decode(image));
                        using (var outStream = OpenWriteSingle(task, file, i => $"{i}.png"))
                            _pngStreamWriter.Write(outStream, bitmap);
                    }
                });

                return true;
            });
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