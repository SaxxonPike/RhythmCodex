using System.Collections.Generic;
using RhythmCodex.Charting;
using RhythmCodex.Charting.Models;
using RhythmCodex.Stepmania.Model;

namespace RhythmCodex.Stepmania.Converters
{
    public interface INoteEncoder
    {
        IEnumerable<Note> Encode(IEnumerable<IEvent> events);
    }
}