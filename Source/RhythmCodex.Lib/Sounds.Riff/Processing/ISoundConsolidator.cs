using System.Collections.Generic;
using RhythmCodex.Charts.Models;
using RhythmCodex.Sounds.Models;

namespace RhythmCodex.Sounds.Riff.Processing;

public interface ISoundConsolidator
{
    void Consolidate(IEnumerable<Sound> sounds, IEnumerable<Event> events);
}