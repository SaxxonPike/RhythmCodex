using RhythmCodex.Infrastructure;

namespace RhythmCodex.Arc.Model;

[Model]
public class ArcEntry
{
    public int NameOffset { get; set; }
    public int Offset { get; set; }
    public int DecompressedSize { get; set; }
    public int CompressedSize { get; set; }
}