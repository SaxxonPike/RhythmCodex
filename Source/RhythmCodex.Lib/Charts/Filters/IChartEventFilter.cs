using System.Collections.Generic;
using RhythmCodex.Charts.Models;

namespace RhythmCodex.Charts.Filters
{
    public interface IChartEventFilter
    {
        IList<Event> GetBpms(IEnumerable<Event> events);
        IList<Event> GetNotes(IEnumerable<Event> events);
        IList<Event> GetTriggers(IEnumerable<Event> events);
    }
}