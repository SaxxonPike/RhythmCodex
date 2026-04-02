using RhythmCodex.Charts.Models;
using RhythmCodex.Games.Beatmania.Ps2.Models;

namespace RhythmCodex.Games.Beatmania.Ps2.Converters;

public interface IBeatmaniaPs2ChartDecoder
{
    Chart Decode(BeatmaniaPs2Chart chart);
}