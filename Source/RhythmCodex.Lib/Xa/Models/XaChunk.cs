using RhythmCodex.Infrastructure;

namespace RhythmCodex.Xa.Models;

[Model]
public class XaChunk
{
    public required byte[] Data { get; set; }
    public required int Channels { get; set; }
    public required int Rate { get; set; }
}