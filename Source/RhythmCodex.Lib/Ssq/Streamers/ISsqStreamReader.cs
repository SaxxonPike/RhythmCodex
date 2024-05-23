using System.Collections.Generic;
using System.IO;
using RhythmCodex.Ssq.Model;

namespace RhythmCodex.Ssq.Streamers;

public interface ISsqStreamReader
{
    List<SsqChunk> Read(Stream stream);
}