using System.Collections.Generic;
using RhythmCodex.Charts.Models;
using RhythmCodex.Games.Stepmania.Model;

namespace RhythmCodex.Games.Stepmania.Converters;

public interface INoteDecoder
{
    List<Event> Decode(IEnumerable<Note> events, int columns);
}