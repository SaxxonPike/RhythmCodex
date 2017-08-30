using System.Collections.Generic;

namespace RhythmCodex.Djmain.Model
{
    public class DjmainSampleSet : IDjmainSampleSet
    {
        public int DataOffset { get; set; }
        public IDictionary<int, DjmainSample> Samples { get; set; }
    }
}
