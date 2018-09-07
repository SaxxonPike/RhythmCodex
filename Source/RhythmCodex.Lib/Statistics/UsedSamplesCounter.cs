using System.Collections.Generic;
using RhythmCodex.Charting;
using RhythmCodex.Infrastructure;

namespace RhythmCodex.Statistics
{
    [Service]
    public class UsedSamplesCounter : IUsedSamplesCounter
    {
        public ISet<int> GetUsedSamples(IEnumerable<IEvent> events)
        {
            throw new System.NotImplementedException();
        }
    }
}