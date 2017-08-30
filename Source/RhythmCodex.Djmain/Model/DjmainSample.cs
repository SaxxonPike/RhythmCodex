using System.Collections.Generic;

namespace RhythmCodex.Djmain.Model
{
    public class DjmainSample : IDjmainSample
    {
        public IDjmainSampleInfo Info { get; set; }
        public IList<byte> Data { get; set; }
    }
}
