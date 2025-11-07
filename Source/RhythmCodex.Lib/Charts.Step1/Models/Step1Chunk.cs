using System;
using RhythmCodex.Infrastructure;

namespace RhythmCodex.Charts.Step1.Models;

[Model]
public class Step1Chunk
{
    public Memory<byte> Data { get; set; }
}