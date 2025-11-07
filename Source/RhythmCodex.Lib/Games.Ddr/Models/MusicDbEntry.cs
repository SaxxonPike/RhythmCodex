namespace RhythmCodex.Games.Ddr.Models;

public class MusicDbEntry
{
    public int? Mcode { get; set; }
    public string? BaseName { get; set; }
    public string? Title { get; set; }
    public string? TitleYomi { get; set; }
    public string? Artist { get; set; }
    public int? BpmMin { get; set; }
    public int? BpmMax { get; set; }
    public int? Series { get; set; }
    public int? Movie { get; set; }
    public int? BgStage { get; set; }
    public int? BemaniFlag { get; set; }
    public int? EventNo { get; set; }
    public int? Limited { get; set; }
    public int? Lamp { get; set; }
    public int? MovieOffset { get; set; }
    public int[] DiffLv { get; set; } = [];
    public int? GenreFlag { get; set; }
    public int? Region { get; set; }
    public int? LimitedCha { get; set; }
}