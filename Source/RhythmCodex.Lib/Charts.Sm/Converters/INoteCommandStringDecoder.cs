using System.Collections.Generic;
using RhythmCodex.Charts.Sm.Model;

namespace RhythmCodex.Charts.Sm.Converters;

public interface INoteCommandStringDecoder
{
    List<Note> Decode(int columns, string notes);
}