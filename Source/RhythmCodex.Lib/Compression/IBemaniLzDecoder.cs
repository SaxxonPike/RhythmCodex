using System.IO;

namespace RhythmCodex.Compression
{
    public interface IBemaniLzDecoder
    {
        byte[] Decode(Stream source);
    }
}