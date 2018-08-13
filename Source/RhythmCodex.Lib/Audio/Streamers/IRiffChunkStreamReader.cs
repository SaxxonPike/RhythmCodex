using System.IO;
using RhythmCodex.Audio.Models;

namespace RhythmCodex.Audio.Streamers
{
    public interface IRiffChunkStreamReader
    {
        IRiffChunk Read(Stream stream);
    }
}