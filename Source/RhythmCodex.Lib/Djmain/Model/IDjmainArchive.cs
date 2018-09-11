using System.Collections.Generic;
using RhythmCodex.Charting;
using RhythmCodex.Infrastructure.Models;
using RhythmCodex.Riff;

namespace RhythmCodex.Djmain.Model
{
    public interface IDjmainArchive
    {
        int Id { get; }
        IList<IChart> Charts { get; }
        IList<ISound> Samples { get; }
    }
}