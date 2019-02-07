using System.Collections.Generic;
using RhythmCodex.Charting;
using RhythmCodex.Infrastructure;
using RhythmCodex.Infrastructure.Models;

namespace RhythmCodex.Archives.Models
{
    [Model]
    public class Archive : IArchive
    {
        public IList<IChart> Charts { get; set; }
        public IList<ISound> Sounds { get; set; }
    }
}