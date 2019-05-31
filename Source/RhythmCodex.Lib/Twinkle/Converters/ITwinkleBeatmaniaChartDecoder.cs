using System;
using System.Collections.Generic;
using RhythmCodex.Beatmania.Models;
using RhythmCodex.Charting.Models;

namespace RhythmCodex.Twinkle.Converters
{
    public interface ITwinkleBeatmaniaChartDecoder
    {
        IList<BeatmaniaPc1Event> Decode(ReadOnlySpan<byte> data, int length);
    }
}