using System.Collections.Generic;
using System.IO;
using RhythmCodex.Archs.Djmain.Model;

namespace RhythmCodex.Archs.Djmain.Converters;

public interface IDjmainSampleDecoder
{
    Dictionary<int, DjmainSample> Decode(Stream stream, IEnumerable<KeyValuePair<int, DjmainSampleInfo>> infos,
        int sampleOffset);
}