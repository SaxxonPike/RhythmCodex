﻿using System.Collections.Generic;
using RhythmCodex.Stepmania.Model;

namespace RhythmCodex.Stepmania.Converters
{
    public interface INoteCommandStringDecoder
    {
        IEnumerable<Note> Decode(int columns, string notes);
    }
}