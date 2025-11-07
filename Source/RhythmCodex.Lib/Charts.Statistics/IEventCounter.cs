using System.Collections.Generic;
using RhythmCodex.Charts.Models;

namespace RhythmCodex.Charts.Statistics;

public interface IEventCounter
{
    int CountCombos(ICollection<Event> events);
    int CountComboFreezes(ICollection<Event> events);
    int CountComboShocks(ICollection<Event> events);
}