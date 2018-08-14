using System.Collections.Generic;
using RhythmCodex.Charting;

namespace RhythmCodex.Audio.Processing
{
    public interface ISoundConsolidator
    {
        void Consolidate(IEnumerable<ISound> sounds, IEnumerable<IEvent> events);
    }
}