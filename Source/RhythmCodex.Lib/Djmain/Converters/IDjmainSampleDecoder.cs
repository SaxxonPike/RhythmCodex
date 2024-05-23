using System.Collections.Generic;
using System.IO;
using RhythmCodex.Djmain.Model;

namespace RhythmCodex.Djmain.Converters;

public interface IDjmainSampleDecoder
{
    IDictionary<int, IDjmainSample> Decode(Stream stream, IEnumerable<KeyValuePair<int, IDjmainSampleInfo>> infos,
        int sampleOffset);
}