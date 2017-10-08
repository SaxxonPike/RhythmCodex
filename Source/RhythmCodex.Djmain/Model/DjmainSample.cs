using System.Collections.Generic;
using RhythmCodex.Infrastructure;

namespace RhythmCodex.Djmain.Model
{
    [Model]
    public class DjmainSample : IDjmainSample
    {
        public IDjmainSampleInfo Info { get; set; }
        public IList<byte> Data { get; set; }
    }
}