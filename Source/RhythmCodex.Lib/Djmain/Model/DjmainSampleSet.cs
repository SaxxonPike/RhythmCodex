using System.Collections.Generic;
using RhythmCodex.Infrastructure;

namespace RhythmCodex.Djmain.Model;

[Model]
public class DjmainSampleSet
{
    public int DataOffset { get; set; }
    public Dictionary<int, DjmainSample> Samples { get; set; }
        
    public override string ToString() => Json.Serialize(this);
}