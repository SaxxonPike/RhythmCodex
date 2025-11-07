using System;
using System.Collections.Generic;
using RhythmCodex.Archs.Twinkle.Model;

namespace RhythmCodex.Archs.Twinkle.Converters;

public interface ITwinkleBeatmaniaChartDecoder
{
    List<TwinkleBeatmaniaChartEvent> Decode(ReadOnlySpan<byte> data);
    int[] GetNoteCounts(ReadOnlySpan<byte> data);
}