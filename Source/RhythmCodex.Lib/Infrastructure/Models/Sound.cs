using System.Collections.Generic;
using RhythmCodex.Attributes;

namespace RhythmCodex.Infrastructure.Models
{
    [Model]
    public class Sound : Metadata, ISound
    {
        public IList<ISample> Samples { get; set; }
    }
}