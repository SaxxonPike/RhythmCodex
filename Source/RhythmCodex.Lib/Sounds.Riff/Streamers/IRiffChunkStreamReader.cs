using System.IO;
using RhythmCodex.Sounds.Riff.Models;

namespace RhythmCodex.Sounds.Riff.Streamers;

public interface IRiffChunkStreamReader
{
    RiffChunk Read(Stream stream);
}