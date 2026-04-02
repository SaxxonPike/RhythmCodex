using System.Collections.Generic;
using RhythmCodex.Infrastructure;

namespace RhythmCodex.Games.Beatmania.Ps2.Models;

[Model]
public class BeatmaniaPs2Chart
{
    public Dictionary<int, int> NoteCounts { get; set; } = [];
    public BigRational Rate { get; set; }
    public List<BeatmaniaPs2Event> Events { get; set; } = [];
}