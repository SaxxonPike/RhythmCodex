using System.IO;
using RhythmCodex.Ssq.Model;

namespace RhythmCodex.Ssq.Streamers
{
    public interface IChunkStreamWriter
    {
        void Write(Stream stream, IChunk chunk);
    }
}