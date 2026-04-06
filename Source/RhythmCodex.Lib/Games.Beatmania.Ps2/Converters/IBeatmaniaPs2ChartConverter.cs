using RhythmCodex.Charts.Models;
using RhythmCodex.Games.Beatmania.Ps2.Models;

namespace RhythmCodex.Games.Beatmania.Ps2.Converters;

/// <summary>
/// Converts beatmaniaIIDX PS2 charts between formats.
/// </summary>
public interface IBeatmaniaPs2ChartConverter
{
    /// <summary>
    /// Converts a chart to the common format.
    /// </summary>
    Chart Convert(BeatmaniaPs2Chart chart);
}