using System.Collections.Generic;
using RhythmCodex.Charting.Models;

namespace RhythmCodex.Statistics
{
    public interface IEventCounter
    {
        int CountCombos(IEnumerable<Event> events);
        int CountComboFreezes(IEnumerable<Event> events);
        int CountComboShocks(IEnumerable<Event> events);
    }
}