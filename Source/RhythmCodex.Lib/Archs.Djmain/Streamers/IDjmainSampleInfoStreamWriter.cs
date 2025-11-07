using System.Collections.Generic;
using System.IO;
using RhythmCodex.Archs.Djmain.Model;

namespace RhythmCodex.Archs.Djmain.Streamers;

public interface IDjmainSampleInfoStreamWriter
{
    void Write(Stream stream, IEnumerable<KeyValuePair<int, DjmainSampleInfo>> definitions);
}