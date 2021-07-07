using System.Collections.Generic;
using RhythmCodex.Charting.Models;
using RhythmCodex.Sounds.Models;

namespace RhythmCodex.Twinkle.Model
{
    public class TwinkleArchive
    {
        public int Id { get; set; }
        public IList<Chart> Charts { get; set; }
        public IList<ISound> Samples { get; set; }
    }
}