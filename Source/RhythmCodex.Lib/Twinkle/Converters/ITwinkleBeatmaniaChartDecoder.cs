using System;
using System.Collections.Generic;
using RhythmCodex.Twinkle.Model;

namespace RhythmCodex.Twinkle.Converters
{
    public interface ITwinkleBeatmaniaChartDecoder
    {
        IList<TwinkleBeatmaniaChartEvent> Decode(ReadOnlySpan<byte> data, int length);
    }
}