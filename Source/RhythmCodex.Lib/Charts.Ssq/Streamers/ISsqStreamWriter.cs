using System.Collections.Generic;
using System.IO;
using RhythmCodex.Charts.Ssq.Model;

namespace RhythmCodex.Charts.Ssq.Streamers;

public interface ISsqStreamWriter
{
    void Write(Stream stream, IEnumerable<SsqChunk> chunks);
}