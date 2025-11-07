using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using RhythmCodex.Charts.Ssq.Model;

namespace RhythmCodex.Charts.Ssq.Converters;

[PublicAPI]
public interface ITriggerChunkEncoder
{
    Memory<byte> Convert(IEnumerable<Trigger> triggers);
}