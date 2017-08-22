using System.Collections.Generic;
using RhythmCodex.Attributes;

namespace RhythmCodex.Audio
{
    public class Sample : Metadata, ISample
    {
        public IList<float> Data { get; set; }
    }
}
