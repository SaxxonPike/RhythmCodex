using System;
using System.Collections.Generic;
using RhythmCodex.Charts.Ssq.Model;

namespace RhythmCodex.Charts.Step1.Converters;

public interface IStep1StepChunkDecoder
{
    List<Step> Convert(ReadOnlySpan<byte> data);
}