using System.Collections.Generic;
using RhythmCodex.Stepmania.Model;

namespace RhythmCodex.Stepmania.Converters;

public interface INoteCommandStringDecoder
{
    List<Note> Decode(int columns, string notes);
}