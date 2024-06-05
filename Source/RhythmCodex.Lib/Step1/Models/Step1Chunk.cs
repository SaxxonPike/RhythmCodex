using System;
using RhythmCodex.Infrastructure;

namespace RhythmCodex.Step1.Models;

[Model]
public class Step1Chunk
{
    public Memory<byte> Data { get; set; }
}