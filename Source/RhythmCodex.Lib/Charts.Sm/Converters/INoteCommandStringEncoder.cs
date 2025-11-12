using System.Collections.Generic;
using RhythmCodex.Charts.Sm.Model;

namespace RhythmCodex.Charts.Sm.Converters;

public interface INoteCommandStringEncoder
{
    string Encode(IEnumerable<Note> notes);
}