using System.Collections.Generic;
using RhythmCodex.Infrastructure;

namespace RhythmCodex.Djmain.Model;

[Model]
public record DjmainSampleSet
{
    public int DataOffset { get; init; }
    public Dictionary<int, DjmainSample> Samples { get; init; } = new();
        
    public override string ToString() => Json.Serialize(this);
}