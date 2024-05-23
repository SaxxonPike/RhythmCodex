using System.Collections.Generic;
using RhythmCodex.Charting.Models;

namespace RhythmCodex.Statistics;

public interface IEventCounter
{
    int CountCombos(IEnumerable<IEvent> events);
    int CountComboFreezes(IEnumerable<IEvent> events);
    int CountComboShocks(IEnumerable<IEvent> events);
}