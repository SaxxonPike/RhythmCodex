using System.Collections.Generic;
using RhythmCodex.Charting;
using RhythmCodex.Charting.Models;
using RhythmCodex.Infrastructure.Models;
using RhythmCodex.Sounds.Models;

namespace RhythmCodex.Archives.Models
{
    public interface IArchive
    {
        IList<IChart> Charts { get; }
        IList<ISound> Sounds { get; }
    }
}