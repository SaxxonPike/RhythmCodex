using System.IO;

namespace RhythmCodex.Compression;

public interface ILzwStreamFactory
{
    Stream Create(Stream source);
}