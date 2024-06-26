using System.Collections.Generic;
using RhythmCodex.Charting.Models;
using RhythmCodex.Sounds.Models;

namespace RhythmCodex.Riff.Processing;

public interface ISoundConsolidator
{
    void Consolidate(IEnumerable<Sound> sounds, IEnumerable<Event> events);
}