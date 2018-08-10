using System.Collections.Generic;
using RhythmCodex.Djmain.Model;

namespace RhythmCodex.Djmain.Converters
{
    public interface IDjmainSampleDecoder
    {
        IDictionary<int, IDjmainSample> Decode(byte[] data, IEnumerable<KeyValuePair<int, IDjmainSampleInfo>> infos,
            int sampleOffset);
    }
}