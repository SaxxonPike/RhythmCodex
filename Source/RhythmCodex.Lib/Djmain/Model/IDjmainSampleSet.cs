using System.Collections.Generic;

namespace RhythmCodex.Djmain.Model;

public interface IDjmainSampleSet
{
    int DataOffset { get; }
    IDictionary<int, DjmainSample> Samples { get; }
}