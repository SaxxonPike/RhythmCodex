using System;
using RhythmCodex.Infrastructure;

namespace RhythmCodex.Archs.Xbox.Model;

[Model]
public class XboxKasEntry
{
    public int Block { get; set; }
    public int Offset { get; set; }
    public Memory<byte> Data { get; set; }
}