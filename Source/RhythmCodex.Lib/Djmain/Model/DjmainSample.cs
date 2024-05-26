using System;
using RhythmCodex.Infrastructure;

namespace RhythmCodex.Djmain.Model;

[Model]
public record DjmainSample
{
    public DjmainSampleInfo Info { get; init; }

    public Memory<byte> Data { get; init; }

    public override string ToString() => Json.Serialize(this);
}