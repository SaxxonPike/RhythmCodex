using System.Collections.Generic;
using RhythmCodex.Charts.Models;
using RhythmCodex.Sounds.Models;

namespace RhythmCodex.Sounds.Riff.Processing;

/// <summary>
/// Consolidates sample data.
/// </summary>
public interface ISoundConsolidator
{
    /// <summary>
    /// Merges BGM-only sounds that are strictly played as stereo pairs, and then
    /// cleans up the chart data to remove any unnecessary sound events from the charts.
    /// </summary>
    void Consolidate(IEnumerable<Sound> sounds, IEnumerable<Chart> charts);
}