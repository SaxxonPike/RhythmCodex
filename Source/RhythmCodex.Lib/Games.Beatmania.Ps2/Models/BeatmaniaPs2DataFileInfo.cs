namespace RhythmCodex.Games.Beatmania.Ps2.Models;

public record BeatmaniaPs2DataFileInfo
{
    public int InfoSize { get; set; }
    public long Offset { get; set; }
    public long Size { get; set; }
}