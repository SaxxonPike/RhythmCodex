using System.Collections.Generic;
using RhythmCodex.Infrastructure;

namespace RhythmCodex.Djmain.Model;

[Model]
public class DjmainSampleSet : IDjmainSampleSet
{
    public int DataOffset { get; set; }
    public IDictionary<int, DjmainSample> Samples { get; set; }
        
    public override string ToString() => Json.Serialize(this);
}