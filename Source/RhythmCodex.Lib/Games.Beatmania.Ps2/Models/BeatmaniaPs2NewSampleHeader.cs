namespace RhythmCodex.Games.Beatmania.Ps2.Models;

public record BeatmaniaPs2NewSampleHeader
{
    // 0x00
    public int SampleCount { get; set; }
    
    // 0x04
    public int TotalSize { get; set; }
    
    // 0x08
    public int Unknown08 { get; set; }
    
    // 0x0C
    public int Unknown0C { get; set; }
}