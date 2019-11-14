using System.Collections.Generic;
using RhythmCodex.Stepmania.Model;

namespace RhythmCodex.Stepmania.Converters
{
    public interface INoteCommandStringDecoder
    {
        IList<Note> Decode(int columns, string notes);
    }
}