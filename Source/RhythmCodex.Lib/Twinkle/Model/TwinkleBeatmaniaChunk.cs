using RhythmCodex.Infrastructure;

namespace RhythmCodex.Twinkle.Model;

[Model]
public class TwinkleBeatmaniaChunk
{
    public byte[] Data { get; set; }
    public int Index { get; set; }
    public long Offset { get; set; }
}