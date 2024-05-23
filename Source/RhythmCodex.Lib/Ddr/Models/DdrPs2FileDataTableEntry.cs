using RhythmCodex.Infrastructure;

namespace RhythmCodex.Ddr.Models;

[Model]
public class DdrPs2FileDataTableEntry
{
    public int Index { get; set; }
    public byte[] Data { get; set; }
}