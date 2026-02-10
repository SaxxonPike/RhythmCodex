using System.Collections.Generic;
using System.IO;
using RhythmCodex.Archs.Firebeat.Models;

namespace RhythmCodex.Archs.Firebeat.Streamers;

public interface IFirebeatChunkStreamReader
{
    IEnumerable<FirebeatChunk> Read(Stream stream);
}