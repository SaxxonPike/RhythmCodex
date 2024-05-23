using System.Collections.Generic;
using System.IO;
using RhythmCodex.Djmain.Model;

namespace RhythmCodex.Djmain.Converters;

public interface IDjmainSampleDecoder
{
    Dictionary<int, DjmainSample> Decode(Stream stream, IEnumerable<KeyValuePair<int, DjmainSampleInfo>> infos,
        int sampleOffset);
}