using System;
using RhythmCodex.Games.Beatmania.Ps2.Models;

namespace RhythmCodex.Games.Beatmania.Ps2.Converters;

public interface IBeatmaniaPs2NewChartDecoder
{
    BeatmaniaPs2Chart Decode(ReadOnlySpan<byte> data);
}