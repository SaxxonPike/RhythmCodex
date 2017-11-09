using System.Collections.Generic;
using RhythmCodex.Charting;
using RhythmCodex.Stepmania.Model;

namespace RhythmCodex.Stepmania.Converters
{
    public interface INoteDecoder
    {
        IEnumerable<IEvent> Decode(IEnumerable<Note> events, int columns);
    }
}