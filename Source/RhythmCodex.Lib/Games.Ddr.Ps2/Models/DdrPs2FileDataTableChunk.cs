using System;

namespace RhythmCodex.Games.Ddr.Ps2.Models;

public class DdrPs2FileDataTableChunk
{
    public bool HasHeaders { get; set; }
    public Memory<byte> Data { get; set; }
}