using System.Collections.Generic;
using RhythmCodex.Charting;
using RhythmCodex.Charting.Models;
using RhythmCodex.Infrastructure.Models;
using RhythmCodex.Riff;
using RhythmCodex.Sounds.Models;

namespace RhythmCodex.Djmain.Model
{
    public class DjmainArchive : IDjmainArchive
    {
        public int Id { get; set; }
        public IList<IChart> Charts { get; set; }
        public IList<ISound> Samples { get; set; }
    }
}