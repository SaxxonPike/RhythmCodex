using System.Collections.Generic;
using RhythmCodex.Audio;
using RhythmCodex.Charting;

namespace RhythmCodex.Djmain.Converters
{
    public class DjmainSampleMapConsolidator : IDjmainSampleMapConsolidator
    {
        public (IDictionary<int, ISound> sounds, IDictionary<int, IChart> charts) Consolidate(
            IEnumerable<KeyValuePair<int, IEnumerable<KeyValuePair<int, ISound>>>> sounds,
            IEnumerable<KeyValuePair<int, IChart>> charts, 
            IEnumerable<KeyValuePair<int, int>> chartMap)
        {
            throw new System.NotImplementedException();
        }
    }

    public interface IDjmainSampleMapConsolidator
    {
        (IDictionary<int, ISound> sounds, IDictionary<int, IChart> charts) Consolidate(
            IEnumerable<KeyValuePair<int, IEnumerable<KeyValuePair<int, ISound>>>> sounds, 
            IEnumerable<KeyValuePair<int, IChart>> charts,
            IEnumerable<KeyValuePair<int, int>> chartMap);
    }
}