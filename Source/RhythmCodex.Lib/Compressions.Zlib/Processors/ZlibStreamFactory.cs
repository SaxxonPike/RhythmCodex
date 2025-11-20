using System.IO;
using System.IO.Compression;
using RhythmCodex.IoC;

namespace RhythmCodex.Compressions.Zlib.Processors;

[Service]
public class ZlibStreamFactory : IZlibStreamFactory
{
    public Stream Create(Stream source)
    {
        return new ZLibStream(source, CompressionMode.Decompress, true);
    }
}