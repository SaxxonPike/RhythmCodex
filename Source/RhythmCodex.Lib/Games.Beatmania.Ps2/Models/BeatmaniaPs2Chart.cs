using System.Collections.Generic;
using RhythmCodex.Infrastructure;

namespace RhythmCodex.Games.Beatmania.Ps2.Models;

[Model]
public record BeatmaniaPs2Chart
{
    public Dictionary<int, int> NoteCounts { get; set; } = [];
    public BigRational Rate { get; set; }
    public List<BeatmaniaPs2Event> Events { get; set; } = [];
    public BigRational SpeedMult { get; set; } = BigRational.One;
}