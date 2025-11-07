using System.Collections.Generic;
using RhythmCodex.Charts.Models;

namespace RhythmCodex.Charts.Statistics;

public interface IUsedSamplesCounter
{
    ISet<int> GetUsedSamples(IEnumerable<Event> events);
}