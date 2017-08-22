using System.Collections.Generic;

namespace RhythmCodex.Djmain.Model
{
    public struct DjmainSampleSet
    {
        public int DataOffset { get; set; }
        public IDictionary<int, DjmainSample> Samples { get; set; }
    }
}
