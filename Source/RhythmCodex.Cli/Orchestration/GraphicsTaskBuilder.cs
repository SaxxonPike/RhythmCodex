using System.Drawing;
using System.Linq;
using ClientCommon;
using RhythmCodex.Cli.Orchestration.Infrastructure;
using RhythmCodex.Dds.Converters;
using RhythmCodex.Dds.Streamers;
using RhythmCodex.Gdi.Streamers;
using RhythmCodex.Graphics.Converters;
using RhythmCodex.Graphics.Models;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;
using RhythmCodex.Tga.Converters;
using RhythmCodex.Tga.Streamers;
using RhythmCodex.Tim.Converters;
using RhythmCodex.Tim.Streamers;

namespace RhythmCodex.Cli.Orchestration;

[Service(singleInstance: false)]
public class GraphicsTaskBuilder(
    IFileSystem fileSystem,
    ILogger logger,
    IPngStreamWriter pngStreamWriter,
    ITgaStreamReader tgaStreamReader,
    ITgaDecoder tgaDecoder,
    IGraphicDsp graphicDsp,
    IDdsStreamReader ddsStreamReader,
    IDdsBitmapDecoder ddsBitmapDecoder,
    ITimDecoder timDecoder,
    ITimStreamReader timStreamReader)
    : TaskBuilderBase<GraphicsTaskBuilder>(fileSystem, logger)
{
    private readonly ITimStreamReader _timStreamReader = timStreamReader;

    private Bitmap CropImage(Bitmap bitmap)
    {
        if (Args.Options.ContainsKey("+crop_ddr"))
        {
            if (bitmap.Width == 512 && bitmap.Height == 256)
                bitmap = graphicDsp.Snip(bitmap, new Rectangle(0, 0, 320, 200));
            else if (bitmap.Width == 256 && bitmap.Height == 128)
                bitmap = graphicDsp.Snip(bitmap, new Rectangle(0, 0, 256, 80));
            else if (bitmap.Width == 1024 && bitmap.Height == 512)
                bitmap = graphicDsp.Snip(bitmap, new Rectangle(0, 0, 640, 480));
        }

        return bitmap;
    }

    public ITask CreateDecodeTim()
    {
        return Build("Decode TIM Image", task =>
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
                var images = timDecoder.Decode(stream);
                task.Message = "Decoding TIM.";

                if (images.Count > 1)
                {
                    var idx = 0;
                    foreach (var image in images)
                    {
                        var bitmap = CropImage(image);
                        using (var outStream = OpenWriteSingle(task, file, i => $"{i}.{idx}.png"))
                            pngStreamWriter.Write(outStream, bitmap);
                        idx++;
                    }
                }
                else if (images.Count == 1)
                {
                    var bitmap = CropImage(images.Single());
                    using var outStream = OpenWriteSingle(task, file, i => $"{i}.png");
                    pngStreamWriter.Write(outStream, bitmap);                            
                }
            });

            return true;
        });
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
                using var stream = OpenRead(task, file);
                var image = ddsStreamReader.Read(stream, (int) stream.Length);
                task.Message = "Decoding DDS.";
                var bitmap = CropImage(ddsBitmapDecoder.Decode(image));
                using var outStream = OpenWriteSingle(task, file, i => $"{i}.png");
                pngStreamWriter.Write(outStream, bitmap);
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
                using var stream = OpenRead(task, file);
                var image = tgaStreamReader.Read(stream, (int) stream.Length);
                task.Message = "Decoding TGA.";
                var bitmap = CropImage(tgaDecoder.Decode(image));

                using var outStream = OpenWriteSingle(task, file, i => $"{i}.png");
                pngStreamWriter.Write(outStream, bitmap);
            });

            return true;
        });
    }
}