using System.Collections.Generic;
using RhythmCodex.Charts.Models;
using RhythmCodex.Games.Stepmania.Model;

namespace RhythmCodex.Games.Stepmania.Converters;

public interface INoteEncoder
{
    List<Note> Encode(IEnumerable<Event> events);
}