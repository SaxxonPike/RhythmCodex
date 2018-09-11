using System.Collections.Generic;
using RhythmCodex.Charting;
using RhythmCodex.Infrastructure.Models;
using RhythmCodex.Riff;

namespace RhythmCodex.Archives
{
    public interface IArchive
    {
        IList<IChart> Charts { get; }
        IList<ISound> Sounds { get; }
    }
}