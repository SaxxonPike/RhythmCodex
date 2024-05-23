using System.Collections.Generic;
using RhythmCodex.Charting.Models;
using RhythmCodex.Sounds.Models;

namespace RhythmCodex.Djmain.Converters;

public interface IDjmainSampleMapConsolidator
{
    (IDictionary<int, ISound> sounds, IDictionary<int, IChart> charts) Consolidate(
        IEnumerable<KeyValuePair<int, IEnumerable<KeyValuePair<int, ISound>>>> sounds, 
        IEnumerable<KeyValuePair<int, IChart>> charts,
        IEnumerable<KeyValuePair<int, int>> chartMap);
}