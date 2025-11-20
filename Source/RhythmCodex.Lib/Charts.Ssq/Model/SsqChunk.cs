using System;
using RhythmCodex.Infrastructure;

namespace RhythmCodex.Charts.Ssq.Model;

[Model]
public class SsqChunk
{
    public short Parameter0 { get; set; }
    public short Parameter1 { get; set; }
    public Memory<byte> Data { get; set; }
}