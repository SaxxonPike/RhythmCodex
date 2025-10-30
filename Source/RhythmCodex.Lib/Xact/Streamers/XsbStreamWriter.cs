using System.IO;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;
using RhythmCodex.Xact.Model;
using RhythmCodex.Xact.Processors;

namespace RhythmCodex.Xact.Streamers;

[Service]
public class XsbStreamWriter(
    IXsbHeaderStreamWriter xsbHeaderStreamWriter,
    IXsbCueStreamWriter xsbCueStreamWriter,
    IXsbSoundStreamWriter xsbSoundStreamWriter,
    IFcs16Calculator fcs16Calculator,
    ILogger logger)
    : IXsbStreamWriter
{
    private readonly IXsbCueStreamWriter _xsbCueStreamWriter = xsbCueStreamWriter;
    private readonly IXsbSoundStreamWriter _xsbSoundStreamWriter = xsbSoundStreamWriter;
    private readonly ILogger _logger = logger;

    public long Write(Stream stream, XsbFile file)
    {
        var mem = new MemoryStream();
        var writer = new BinaryWriter(mem);

        var header = file.Header;
        xsbHeaderStreamWriter.Write(mem, header);

        // write crc
        var crc = fcs16Calculator.Calculate(mem.AsSpan(18));
        mem.Position = 8;
        writer.Write(crc);

        // write to target stream
        mem.Position = 0;
        mem.CopyTo(stream);
        return mem.Length;
    }
}