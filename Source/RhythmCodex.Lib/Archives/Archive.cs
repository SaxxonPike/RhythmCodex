using System.Collections.Generic;
using RhythmCodex.Audio;
using RhythmCodex.Charting;

namespace RhythmCodex.Archives
{
    public class Archive : IArchive
    {
        public IList<IChart> Charts { get; set; }
        public IList<ISound> Sounds { get; set; }
    }
}