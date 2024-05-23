using System.IO;
using RhythmCodex.Riff.Models;

namespace RhythmCodex.Riff.Streamers;

public interface IRiffChunkStreamReader
{
    IRiffChunk Read(Stream stream);
}