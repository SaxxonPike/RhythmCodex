using System.Collections.Generic;
using RhythmCodex.Charting.Models;
using RhythmCodex.Sounds.Models;

namespace RhythmCodex.Djmain.Converters
{
    public class DjmainSampleMapConsolidator : IDjmainSampleMapConsolidator
    {
        public (IDictionary<int, ISound> sounds, IDictionary<int, Chart> charts) Consolidate(
            IEnumerable<KeyValuePair<int, IEnumerable<KeyValuePair<int, ISound>>>> sounds,
            IEnumerable<KeyValuePair<int, Chart>> charts, 
            IEnumerable<KeyValuePair<int, int>> chartMap)
        {
            throw new System.NotImplementedException();
        }
    }

    public interface IDjmainSampleMapConsolidator
    {
        (IDictionary<int, ISound> sounds, IDictionary<int, Chart> charts) Consolidate(
            IEnumerable<KeyValuePair<int, IEnumerable<KeyValuePair<int, ISound>>>> sounds, 
            IEnumerable<KeyValuePair<int, Chart>> charts,
            IEnumerable<KeyValuePair<int, int>> chartMap);
    }
}