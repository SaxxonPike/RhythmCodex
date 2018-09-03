using System.IO;

namespace RhythmCodex.Compression
{
    public interface IBemaniLzDecoder
    {
        void Decode(Stream source, Stream target);
    }
}