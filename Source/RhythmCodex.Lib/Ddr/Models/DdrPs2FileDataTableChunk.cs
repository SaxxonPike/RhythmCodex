using System;

namespace RhythmCodex.Ddr.Models;

public class DdrPs2FileDataTableChunk
{
    public bool HasHeaders { get; set; }
    public Memory<byte> Data { get; set; }
}