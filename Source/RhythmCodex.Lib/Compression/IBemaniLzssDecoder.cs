using System.IO;

namespace RhythmCodex.Compression
{
    public interface IBemaniLzssDecoder
    {
        byte[] Decode(Stream source);
    }
}