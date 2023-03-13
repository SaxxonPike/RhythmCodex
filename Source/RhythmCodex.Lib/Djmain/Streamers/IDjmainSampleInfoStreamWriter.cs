using System.Collections.Generic;
using System.IO;
using RhythmCodex.Djmain.Model;

namespace RhythmCodex.Djmain.Streamers;

public interface IDjmainSampleInfoStreamWriter
{
    void Write(Stream stream, IEnumerable<KeyValuePair<int, DjmainSampleInfo>> definitions);
}