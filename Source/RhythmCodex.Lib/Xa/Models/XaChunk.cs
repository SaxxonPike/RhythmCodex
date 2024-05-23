using RhythmCodex.Infrastructure;

namespace RhythmCodex.Xa.Models;

[Model]
public class XaChunk
{
    public byte[] Data { get; set; }
    public int Channels { get; set; }
    public int Rate { get; set; }
}