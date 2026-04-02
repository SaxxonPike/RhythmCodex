namespace RhythmCodex.Games.Beatmania.Ps2.Models;

public record BeatmaniaPs2DifficultyInfo
{
    public string? Name { get; set; }
    public int Players { get; set; }
    public int Level { get; set; }
    public int Keysound { get; set; }
    public int Bgm { get; set; }
    public int Movie { get; set; }
    public int Chart { get; set; }
}