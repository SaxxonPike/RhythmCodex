using System.IO;
using RhythmCodex.Ssq.Model;

namespace RhythmCodex.Ssq.Streamers
{
    public interface IChunkStreamReader
    {
        Chunk Read(Stream stream);
    }
}