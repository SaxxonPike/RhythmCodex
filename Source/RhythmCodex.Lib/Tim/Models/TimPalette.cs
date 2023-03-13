using System.Collections.Generic;

namespace RhythmCodex.Tim.Models;

public class TimPalette
{
    public int OriginX { get; set; }
    public int OriginY { get; set; }
    public IList<short> Entries { get; set; }
}