using System.IO;

namespace RhythmCodex.Compression;

public interface IBzip2StreamFactory
{
    Stream Create(Stream source);
}