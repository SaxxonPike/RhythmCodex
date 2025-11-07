using System.IO;
using RhythmCodex.Charts.Ssq.Model;

namespace RhythmCodex.Charts.Ssq.Streamers;

public interface IChunkStreamWriter
{
    void Write(Stream stream, SsqChunk ssqChunk);
    void WriteEnd(Stream stream);
}