﻿using System.Collections.Generic;
using RhythmCodex.Ssq.Model;

namespace RhythmCodex.Ssq.Converters
{
    public interface IStepChunkDecoder
    {
        IList<Step> Convert(byte[] data);
    }
}