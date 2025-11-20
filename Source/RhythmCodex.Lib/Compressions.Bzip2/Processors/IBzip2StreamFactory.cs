using System.IO;

namespace RhythmCodex.Compressions.Bzip2.Processors;

public interface IBzip2StreamFactory
{
    Stream Create(Stream source);
}