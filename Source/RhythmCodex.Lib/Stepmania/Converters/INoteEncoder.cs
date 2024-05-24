using System.Collections.Generic;
using RhythmCodex.Charting.Models;
using RhythmCodex.Stepmania.Model;

namespace RhythmCodex.Stepmania.Converters;

public interface INoteEncoder
{
    List<Note> Encode(IEnumerable<Event> events);
}