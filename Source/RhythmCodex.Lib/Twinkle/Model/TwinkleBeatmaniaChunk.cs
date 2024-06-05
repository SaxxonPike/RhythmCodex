using System;
using RhythmCodex.Infrastructure;

namespace RhythmCodex.Twinkle.Model;

[Model]
public class TwinkleBeatmaniaChunk
{
    public Memory<byte> Data { get; set; }
    public int Index { get; set; }
    public long Offset { get; set; }
}