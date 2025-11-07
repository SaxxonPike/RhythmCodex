using System.Collections.Generic;
using System.IO;
using RhythmCodex.Archs.Djmain.Model;

namespace RhythmCodex.Archs.Djmain.Streamers;

public interface IDjmainChunkStreamReader
{
    IEnumerable<DjmainChunk> Read(Stream stream);
}