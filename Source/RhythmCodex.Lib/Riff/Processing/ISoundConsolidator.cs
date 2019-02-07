using System.Collections.Generic;
using RhythmCodex.Charting;
using RhythmCodex.Charting.Models;
using RhythmCodex.Infrastructure.Models;
using RhythmCodex.Sounds.Models;

namespace RhythmCodex.Riff.Processing
{
    public interface ISoundConsolidator
    {
        void Consolidate(IEnumerable<ISound> sounds, IEnumerable<IEvent> events);
    }
}