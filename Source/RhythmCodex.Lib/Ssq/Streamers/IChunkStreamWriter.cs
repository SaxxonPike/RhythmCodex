using System.IO;
using RhythmCodex.Ssq.Model;

namespace RhythmCodex.Ssq.Streamers
{
    public interface IChunkStreamWriter
    {
        void Write(Stream stream, Chunk chunk);
    }
}