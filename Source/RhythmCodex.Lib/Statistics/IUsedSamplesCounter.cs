using System.Collections.Generic;
using RhythmCodex.Charting;

namespace RhythmCodex.Statistics
{
    public interface IUsedSamplesCounter
    {
        ISet<int> GetUsedSamples(IEnumerable<IEvent> events);
    }
}