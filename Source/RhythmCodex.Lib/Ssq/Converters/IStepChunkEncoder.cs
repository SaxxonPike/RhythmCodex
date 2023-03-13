using System.Collections.Generic;
using RhythmCodex.Ssq.Model;

namespace RhythmCodex.Ssq.Converters;

public interface IStepChunkEncoder
{
    byte[] Convert(IEnumerable<Step> steps);
}