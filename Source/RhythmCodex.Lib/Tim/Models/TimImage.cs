using System;
using System.Collections.Generic;

namespace RhythmCodex.Tim.Models;

public class TimImage
{
    public int ImageType { get; set; }
    public List<TimPalette> Cluts { get; set; }
    public int OriginX { get; set; }
    public int OriginY { get; set; }
    public int Stride { get; set; }
    public int Height { get; set; }
    public ReadOnlyMemory<byte> Data { get; set; }
}