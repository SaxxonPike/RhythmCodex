using System.Collections.Generic;

namespace RhythmCodex.Games.Ddr;

public class DdrSongInfo
{
    public string? Title { get; set; }
    public string? Subtitle { get; set; }
    public string? Artist { get; set; }
    public List<int> Bpms { get; set; } = [];
    public Dictionary<string, int> Levels { get; set; } = new();
}