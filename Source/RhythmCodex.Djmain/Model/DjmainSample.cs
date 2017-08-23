using System.Collections.Generic;

namespace RhythmCodex.Djmain.Model
{
    public struct DjmainSample
    {
        public DjmainSampleInfo Info { get; set; }
        public IList<byte> Data { get; set; }
    }
}
