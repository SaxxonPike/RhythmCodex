using System.Collections.Generic;
using System.IO;
using RhythmCodex.Archs.Djmain.Model;

namespace RhythmCodex.Archs.Djmain.Streamers;

public interface IDjmainSampleInfoStreamReader
{
    Dictionary<int, DjmainSampleInfo> Read(Stream stream);
}