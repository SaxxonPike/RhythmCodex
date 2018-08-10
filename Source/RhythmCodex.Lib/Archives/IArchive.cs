using System.Collections.Generic;
using RhythmCodex.Audio;
using RhythmCodex.Charting;

namespace RhythmCodex.Archives
{
    public interface IArchive
    {
        IList<IChart> Charts { get; }
        IList<ISound> Sounds { get; }
    }
}