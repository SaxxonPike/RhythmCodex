using System.Collections.Generic;
using System.Linq;
using RhythmCodex.Charting.Models;
using RhythmCodex.IoC;
using RhythmCodex.Meta.Models;

namespace RhythmCodex.Charting.Filters
{
    /// <summary>
    /// Filters out events.
    /// </summary>
    [Service]
    public class ChartEventFilter : IChartEventFilter
    {
        public IList<Event> GetBpms(IEnumerable<Event> events)
        {
            return events
                .Where(e => e[NumericData.Bpm] != null || e[NumericData.Stop] != null)
                .ToList();
        }

        public IList<Event> GetNotes(IEnumerable<Event> events)
        {
            return events
                .Where(e => e[FlagData.Note] == true)
                .ToList();
        }

        public IList<Event> GetTriggers(IEnumerable<Event> events)
        {
            return events
                .Where(e => e[NumericData.Trigger] != null)
                .ToList();
        }
    }
}