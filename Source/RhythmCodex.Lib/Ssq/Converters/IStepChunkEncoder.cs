using System.Collections.Generic;
using JetBrains.Annotations;
using RhythmCodex.Ssq.Model;

namespace RhythmCodex.Ssq.Converters;

[PublicAPI]
public interface IStepChunkEncoder
{
    byte[] Convert(IReadOnlyCollection<Step> steps);
}