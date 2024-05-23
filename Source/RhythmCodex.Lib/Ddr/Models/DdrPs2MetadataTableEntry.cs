using RhythmCodex.Infrastructure;

namespace RhythmCodex.Ddr.Models;

[Model]
public class DdrPs2MetadataTableEntry
{
    public required int Index { get; set; }
    public required byte[] Data { get; set; }
}