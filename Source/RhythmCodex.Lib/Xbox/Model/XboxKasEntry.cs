using System;
using RhythmCodex.Infrastructure;

namespace RhythmCodex.Xbox.Model;

[Model]
public class XboxKasEntry
{
    public int Block { get; init; }
    public int Offset { get; init; }
    public Memory<byte> Data { get; init; }
}