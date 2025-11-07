using System.IO;
using RhythmCodex.Charts.Ssq.Model;

namespace RhythmCodex.Charts.Ssq.Streamers;

public interface IChunkStreamReader
{
    SsqChunk? Read(Stream stream);
}