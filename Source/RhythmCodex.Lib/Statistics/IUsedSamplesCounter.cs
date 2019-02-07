using System.Collections.Generic;
using RhythmCodex.Charting;
using RhythmCodex.Charting.Models;

namespace RhythmCodex.Statistics
{
    public interface IUsedSamplesCounter
    {
        ISet<int> GetUsedSamples(IEnumerable<IEvent> events);
    }
}