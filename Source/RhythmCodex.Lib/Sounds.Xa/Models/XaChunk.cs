using System;
using RhythmCodex.Infrastructure;

namespace RhythmCodex.Sounds.Xa.Models;

[Model]
public class XaChunk
{
    public Memory<byte> Data { get; set; }
    public int Channels { get; set; }
    public int Rate { get; set; }
}