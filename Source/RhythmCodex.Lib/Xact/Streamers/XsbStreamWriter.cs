using System.IO;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;
using RhythmCodex.Xact.Model;
using RhythmCodex.Xact.Processors;

namespace RhythmCodex.Xact.Streamers
{
    [Service]
    public class XsbSoundStreamWriter : IXsbSoundStreamWriter
    {
    }
    
    [Service]
    public class XsbCueStreamWriter : IXsbCueStreamWriter
    {
    }
    
    [Service]
    public class XsbStreamWriter : IXsbStreamWriter
    {
        private readonly IXsbHeaderStreamWriter _xsbHeaderStreamWriter;
        private readonly IXsbCueStreamWriter _xsbCueStreamWriter;
        private readonly IXsbSoundStreamWriter _xsbSoundStreamWriter;
        private readonly IFcs16Calculator _fcs16Calculator;
        private readonly ILogger _logger;

        public XsbStreamWriter(
            IXsbHeaderStreamWriter xsbHeaderStreamWriter,
            IXsbCueStreamWriter xsbCueStreamWriter,
            IXsbSoundStreamWriter xsbSoundStreamWriter,
            IFcs16Calculator fcs16Calculator,
            ILogger logger)
        {
            _xsbHeaderStreamWriter = xsbHeaderStreamWriter;
            _xsbCueStreamWriter = xsbCueStreamWriter;
            _xsbSoundStreamWriter = xsbSoundStreamWriter;
            _fcs16Calculator = fcs16Calculator;
            _logger = logger;
        }

        public long Write(Stream stream, XsbFile file)
        {
            var mem = new MemoryStream();
            var writer = new BinaryWriter(mem);

            var header = file.Header;
            _xsbHeaderStreamWriter.Write(mem, header);

            // write crc
            var crc = _fcs16Calculator.Calculate(mem.AsMemory().Span.Slice(18));
            mem.Position = 8;
            writer.Write(crc);

            // write to target stream
            mem.Position = 0;
            mem.CopyTo(stream);
            return mem.Length;
        }
    }
}