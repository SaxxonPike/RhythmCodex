using System.Collections.Generic;

namespace RhythmCodex.Beatmania.Models;

public class BeatmaniaPc1Chart
{
    public int Index { get; set; }
    public IList<BeatmaniaPc1Event> Data { get; set; } = [];
}