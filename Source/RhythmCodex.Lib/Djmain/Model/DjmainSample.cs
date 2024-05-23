using System;
using RhythmCodex.Infrastructure;

namespace RhythmCodex.Djmain.Model;

[Model]
public class DjmainSample
{
    public required DjmainSampleInfo Info { get; set; }
    public required Memory<byte> Data { get; set; }
        
    public override string ToString() => Json.Serialize(this);
}