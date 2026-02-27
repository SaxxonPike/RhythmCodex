using System;
using RhythmCodex.Infrastructure;

namespace RhythmCodex.Archs.Firebeat.Models;

[Model]
public class FirebeatSample
{
    public FirebeatSampleInfo Info { get; set; }
    public Memory<byte> Data { get; set; } = Memory<byte>.Empty;
}