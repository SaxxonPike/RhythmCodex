using System.Collections.Generic;

namespace RhythmCodex.Beatmania.Models;

public class BeatmaniaPc1Chart
{
    public int Index { get; init; }
    public List<BeatmaniaPc1Event> Data { get; init; } = [];
}