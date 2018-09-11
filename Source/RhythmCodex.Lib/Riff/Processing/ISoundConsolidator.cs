using System.Collections.Generic;
using RhythmCodex.Charting;
using RhythmCodex.Infrastructure.Models;

namespace RhythmCodex.Riff.Processing
{
    public interface ISoundConsolidator
    {
        void Consolidate(IEnumerable<ISound> sounds, IEnumerable<IEvent> events);
    }
}