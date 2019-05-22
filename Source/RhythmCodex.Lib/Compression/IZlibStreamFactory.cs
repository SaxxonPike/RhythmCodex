using System.IO;

namespace RhythmCodex.Compression
{
    public interface IZlibStreamFactory
    {
        Stream Create(Stream source);
    }
}