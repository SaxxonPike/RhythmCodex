using System.Collections.Generic;
using RhythmCodex.Attributes;

namespace RhythmCodex.Audio
{
    public class Sound : Metadata, ISound
    {
        public IList<ISample> Samples { get; set; }
    }
}
