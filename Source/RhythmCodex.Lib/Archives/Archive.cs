using System.Collections.Generic;
using RhythmCodex.Charting;
using RhythmCodex.Infrastructure.Models;
using RhythmCodex.Riff;

namespace RhythmCodex.Archives
{
    public class Archive : IArchive
    {
        public IList<IChart> Charts { get; set; }
        public IList<ISound> Sounds { get; set; }
    }
}