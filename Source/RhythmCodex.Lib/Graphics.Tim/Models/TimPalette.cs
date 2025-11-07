using System.Collections.Generic;

namespace RhythmCodex.Graphics.Tim.Models;

public class TimPalette
{
    public int OriginX { get; set; }
    public int OriginY { get; set; }
    public List<short> Entries { get; set; }
}