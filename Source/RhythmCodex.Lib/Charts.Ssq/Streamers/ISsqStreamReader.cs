using System.Collections.Generic;
using System.IO;
using RhythmCodex.Charts.Ssq.Model;

namespace RhythmCodex.Charts.Ssq.Streamers;

public interface ISsqStreamReader
{
    List<SsqChunk> Read(Stream stream);
}