using System.Collections.Generic;
using RhythmCodex.Charts.Models;
using RhythmCodex.Games.Beatmania.Ps2.Models;
using RhythmCodex.Sounds.Models;

namespace RhythmCodex.Games.Beatmania.Ps2.Converters;

/// <summary>
/// Consolidates PS2 chart sets into a master chart and keysound set.
/// </summary>
public interface IBeatmaniaPs2ChartSetConsolidator
{
    /// <summary>
    /// Consolidates and deduplicates charts and keysounds within a chart set.
    /// Additionally, BGM and keysound set mappings are assigned.
    /// </summary>
    (List<Chart> Charts, List<Sound> Sounds) Consolidate(BeatmaniaPs2ChartSet chartSet);
}