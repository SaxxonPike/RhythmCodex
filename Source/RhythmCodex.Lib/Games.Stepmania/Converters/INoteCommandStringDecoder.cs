using System.Collections.Generic;
using RhythmCodex.Games.Stepmania.Model;

namespace RhythmCodex.Games.Stepmania.Converters;

public interface INoteCommandStringDecoder
{
    List<Note> Decode(int columns, string notes);
}