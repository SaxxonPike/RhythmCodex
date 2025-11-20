using System.Collections.Generic;
using RhythmCodex.Charts.Models;
using RhythmCodex.Sounds.Models;

namespace RhythmCodex.Archs.Djmain.Converters;

public class DjmainSampleMapConsolidator : IDjmainSampleMapConsolidator
{
    public (Dictionary<int, Sound> sounds, Dictionary<int, Chart> charts) Consolidate(
        IEnumerable<KeyValuePair<int, IEnumerable<KeyValuePair<int, Sound>>>> sounds,
        IEnumerable<KeyValuePair<int, Chart>> charts, 
        IEnumerable<KeyValuePair<int, int>> chartMap)
    {
        throw new System.NotImplementedException();
    }
}