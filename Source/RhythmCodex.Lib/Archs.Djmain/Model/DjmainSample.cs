using System;
using RhythmCodex.Infrastructure;

namespace RhythmCodex.Archs.Djmain.Model;

[Model]
public class DjmainSample
{
    public DjmainSampleInfo Info { get; set; }

    public Memory<byte> Data { get; set; }

    public override string ToString() => Json.Serialize(this);
}