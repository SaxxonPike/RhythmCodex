using System.Collections.Generic;
using RhythmCodex.Charting.Models;

namespace RhythmCodex.Statistics;

public interface IEventCounter
{
    int CountCombos(ICollection<Event> events);
    int CountComboFreezes(ICollection<Event> events);
    int CountComboShocks(ICollection<Event> events);
}