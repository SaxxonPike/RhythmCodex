using System.IO;
using System.IO.Compression;
using RhythmCodex.IoC;

namespace RhythmCodex.Compression;

[Service]
public class ZlibStreamFactory : IZlibStreamFactory
{
    public Stream Create(Stream source)
    {
        return new ZLibStream(source, CompressionMode.Decompress, true);
    }
}