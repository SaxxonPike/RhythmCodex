using System.Collections.Generic;
using RhythmCodex.Charting.Models;
using RhythmCodex.Sounds.Models;

namespace RhythmCodex.Djmain.Converters;

public interface IDjmainSampleMapConsolidator
{
    (Dictionary<int, Sound> sounds, Dictionary<int, Chart> charts) Consolidate(
        IEnumerable<KeyValuePair<int, IEnumerable<KeyValuePair<int, Sound>>>> sounds, 
        IEnumerable<KeyValuePair<int, Chart>> charts,
        IEnumerable<KeyValuePair<int, int>> chartMap);
}