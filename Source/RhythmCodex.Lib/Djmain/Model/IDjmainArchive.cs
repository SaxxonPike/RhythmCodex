using System.Collections.Generic;
using RhythmCodex.Charting.Models;
using RhythmCodex.Sounds.Models;

namespace RhythmCodex.Djmain.Model
{
    public interface IDjmainArchive
    {
        int Id { get; }
        IList<IChart> Charts { get; }
        IList<ISound> Samples { get; }
    }
}