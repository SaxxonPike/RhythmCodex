using System.Collections.Generic;
using RhythmCodex.Charting.Models;
using RhythmCodex.Stepmania.Model;

namespace RhythmCodex.Stepmania.Converters
{
    public interface INoteDecoder
    {
        IList<Event> Decode(IEnumerable<Note> events, int columns);
    }
}