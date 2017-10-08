using System.Collections.Generic;
using RhythmCodex.Attributes;
using RhythmCodex.Infrastructure;

namespace RhythmCodex.Audio
{
    [Model]
    public class Sound : Metadata, ISound
    {
        public IList<ISample> Samples { get; set; }
    }
}