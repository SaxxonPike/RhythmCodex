using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using RhythmCodex.Ssq.Model;

namespace RhythmCodex.Ssq.Converters;

[PublicAPI]
public interface ITriggerChunkEncoder
{
    Memory<byte> Convert(IReadOnlyCollection<Trigger> triggers);
}