using System.Collections.Generic;
using RhythmCodex.Charting;
using RhythmCodex.Infrastructure.Models;

namespace RhythmCodex.Archives.Models
{
    public interface IArchive
    {
        IList<IChart> Charts { get; }
        IList<ISound> Sounds { get; }
    }
}