using System.Collections.Generic;

namespace RhythmCodex.Games.Beatmania.Ps2.Models;

public record BeatmaniaPs2SongInfo
{
    public int InfoSize { get; set; }
    public string? Name { get; set; } = "";
    public int NameRef { get; set; }
    public IReadOnlyList<BeatmaniaPs2DifficultyInfo> Difficulties { get; set; } = [];
    public bool IsFiveKey { get; set; }
    public int ChartRef { get; set; }
}