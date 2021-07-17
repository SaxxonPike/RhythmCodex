using System.Collections.Generic;
using RhythmCodex.Charting.Models;

namespace RhythmCodex.Charting.Filters
{
    public interface IChartEventFilter
    {
        IList<Event> GetTempos(IEnumerable<Event> events);
        IList<Event> GetNotes(IEnumerable<Event> events);
        IList<Event> GetTriggers(IEnumerable<Event> events);
    }
}