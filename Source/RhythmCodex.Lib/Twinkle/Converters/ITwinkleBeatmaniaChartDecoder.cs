using System;
using System.Collections.Generic;
using RhythmCodex.Twinkle.Model;

namespace RhythmCodex.Twinkle.Converters;

public interface ITwinkleBeatmaniaChartDecoder
{
    List<TwinkleBeatmaniaChartEvent> Decode(ReadOnlySpan<byte> data, int length);
    int[] GetNoteCounts(ReadOnlySpan<byte> data, int length);
}