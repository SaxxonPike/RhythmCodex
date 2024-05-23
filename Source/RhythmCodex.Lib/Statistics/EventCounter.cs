using System.Collections.Generic;
using System.Linq;
using RhythmCodex.Charting.Models;
using RhythmCodex.Extensions;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;
using RhythmCodex.Meta.Models;

namespace RhythmCodex.Statistics;

[Service]
public class EventCounter : IEventCounter
{
    public int CountCombos(IEnumerable<IEvent> events)
    {
        return GroupEventsByTime(events.AsList())
            .Select(g => g.FirstOrDefault(ev => ev[FlagData.Note] == true))
            .Count(ev => ev != null);
    }

    public int CountComboFreezes(IEnumerable<IEvent> events)
    {
        return GroupEventsByTime(events.AsList())
            .Select(g => g.FirstOrDefault(ev => ev[FlagData.Freeze] == true))
            .Count(ev => ev != null);
    }

    public int CountComboShocks(IEnumerable<IEvent> events)
    {
        return GroupEventsByTime(events.AsList())
            .Select(g => g.FirstOrDefault(ev => ev[FlagData.Shock] == true))
            .Count(ev => ev != null);
    }

    private static IEnumerable<IGrouping<BigRational?, IEvent>> GroupEventsByTime(IList<IEvent> eventList)
    {
        return eventList.All(ev => ev[NumericData.MetricOffset] != null)
            ? eventList.GroupBy(ev => ev[NumericData.MetricOffset])
            : eventList.All(ev => ev[NumericData.LinearOffset] != null)
                ? eventList.GroupBy(ev => ev[NumericData.LinearOffset])
                : throw new RhythmCodexException("Events must be groupable by either metric or linear offsets.");
    }
}