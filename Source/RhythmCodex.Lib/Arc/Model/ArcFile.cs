using RhythmCodex.Infrastructure;

namespace RhythmCodex.Arc.Model;

[Model]
public class ArcFile
{
    public string Name { get; set; }
    public int CompressedSize { get; set; }
    public int DecompressedSize { get; set; }
    public byte[] Data { get; set; }
}