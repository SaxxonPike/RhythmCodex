using System.Collections.Generic;
using RhythmCodex.Attributes;

namespace RhythmCodex.Audio
{
    public class Sample : Metadata, ISample
    {
        public IEnumerable<float> Data { get; set; }
    }
}
