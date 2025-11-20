using System.IO;

namespace RhythmCodex.Compressions.Zlib.Processors;

public interface IZlibStreamFactory
{
    Stream Create(Stream source);
}