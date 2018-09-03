using System.IO;

namespace RhythmCodex.Compression
{
    public interface IBemaniLzss2Decoder
    {
        void DecompressFirebeat(Stream source, Stream target, int length, int decompLength);
        void DecompressGcz(Stream source, Stream target, int length, int decompLength);
    }
}