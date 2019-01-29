using System.IO;

namespace RhythmCodex.Compression
{
    public interface IBemaniLzss2Decoder
    {
        byte[] DecompressFirebeat(Stream source, int length, int decompLength);
        byte[] DecompressGcz(Stream source, int length, int decompLength);
    }
}