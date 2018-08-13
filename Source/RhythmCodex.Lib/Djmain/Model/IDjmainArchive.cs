using System.Collections.Generic;
using RhythmCodex.Audio;
using RhythmCodex.Charting;

namespace RhythmCodex.Djmain.Model
{
    public interface IDjmainArchive
    {
        int Id { get; }
        IList<IChart> Charts { get; }
        IList<ISound> Samples { get; }
    }
}