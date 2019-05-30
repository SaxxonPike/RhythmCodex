using System.IO;
using System.IO.Compression;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;

namespace RhythmCodex.Compression
{
    [Service]
    public class ZlibStreamFactory : IZlibStreamFactory
    {
        public Stream Create(Stream source)
        {
            var format = new byte[2];
            if (source.Read(format, 0, 2) < 2)
                throw new RhythmCodexException("Zlib header not present");

            var check = (format[1] | (format[0] << 8)) % 31;
            if (check != 0)
                throw new RhythmCodexException("Zlib header is corrupt");

            var cinfo = format[0] >> 4;
            if (cinfo > 7)
                throw new RhythmCodexException("Invalid window size");

            var cm = format[0] & 0xF;
            if (cm != 0x8)
                throw new RhythmCodexException($"Unrecognized compression method 0x{cm:X1}");
                
            var flevel = format[1] >> 6;
            
            var fdict = (format[1] & 0x20) != 0;
            if (fdict)
                throw new RhythmCodexException("Don't know how to handle a dictionary yet.");
            
            return new DeflateStream(source, CompressionMode.Decompress, true);
        }
    }
}