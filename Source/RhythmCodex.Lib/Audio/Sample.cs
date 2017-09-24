using System.Collections.Generic;
using RhythmCodex.Attributes;
using RhythmCodex.Infrastructure;

namespace RhythmCodex.Audio
{
    [Model]
    public class Sample : Metadata, ISample
    {
        public IList<float> Data { get; set; }
    }
}