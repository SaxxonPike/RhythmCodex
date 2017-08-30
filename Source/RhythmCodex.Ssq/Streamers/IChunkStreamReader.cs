using System.IO;
using RhythmCodex.Ssq.Model;

namespace RhythmCodex.Ssq.Streamers
{
    public interface IChunkStreamReader
    {
        IChunk Read(Stream stream);
    }
}