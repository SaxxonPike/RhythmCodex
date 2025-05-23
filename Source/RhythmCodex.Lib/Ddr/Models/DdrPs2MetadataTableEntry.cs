using System;
using RhythmCodex.Infrastructure;

namespace RhythmCodex.Ddr.Models;

[Model]
public class DdrPs2MetadataTableEntry
{
    public int Index { get; set; }
    public Memory<byte> Data { get; set; }
}