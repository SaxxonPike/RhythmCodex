using System.Collections.Generic;
using System.IO;
using RhythmCodex.Djmain.Model;

namespace RhythmCodex.Djmain.Streamers;

public interface IDjmainSampleInfoStreamReader
{
    Dictionary<int, DjmainSampleInfo> Read(Stream stream);
}