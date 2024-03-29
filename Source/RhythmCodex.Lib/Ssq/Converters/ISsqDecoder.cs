﻿using System.Collections.Generic;
using RhythmCodex.Charting.Models;
using RhythmCodex.Ssq.Model;

namespace RhythmCodex.Ssq.Converters
{
    public interface ISsqDecoder
    {
        IList<IChart> Decode(IEnumerable<SsqChunk> data);
    }
}