using System.IO;

namespace RhythmCodex.Compressions.Lzw.Processors;

public interface ILzwStreamFactory
{
    Stream Create(Stream source);
}