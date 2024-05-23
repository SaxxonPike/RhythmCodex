﻿using System.Collections.Generic;
using JetBrains.Annotations;
using RhythmCodex.Ssq.Model;

namespace RhythmCodex.Ssq.Converters;

[PublicAPI]
public interface ITimingChunkEncoder
{
    byte[] Convert(IEnumerable<Timing> timings);
}