using System.IO;

namespace RhythmCodex.Compression
{
    public interface IBemaniLzssDecoder
    {
        void Decode(Stream source, Stream target);
    }
}