using System.Collections.Generic;
using RhythmCodex.Stepmania.Model;

namespace RhythmCodex.Stepmania.Converters;

public interface INoteCommandStringEncoder
{
    string Encode(IEnumerable<Note> notes);
}