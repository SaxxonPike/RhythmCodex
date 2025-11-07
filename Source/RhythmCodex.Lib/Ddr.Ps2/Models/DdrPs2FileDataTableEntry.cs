using System;
using RhythmCodex.Infrastructure;

namespace RhythmCodex.Ddr.Ps2.Models;

[Model]
public class DdrPs2FileDataTableEntry
{
    public int Index { get; set; }
    public Memory<byte> Data { get; set; }
}