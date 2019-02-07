using System.Collections.Generic;
using RhythmCodex.Infrastructure;
using RhythmCodex.Meta.Models;

namespace RhythmCodex.Sounds.Models
{
    [Model]
    public class Sound : Metadata, ISound
    {
        public IList<ISample> Samples { get; set; }
    }
}