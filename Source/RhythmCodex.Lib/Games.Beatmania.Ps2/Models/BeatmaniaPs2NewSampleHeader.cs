namespace RhythmCodex.Games.Beatmania.Ps2.Models;

public record struct BeatmaniaPs2NewSampleHeader
{
    // 0x00
    public int SampleCount { get; init; }
    
    // 0x04
    public int TotalSize { get; init; }
    
    // 0x08
    public int Unknown08 { get; init; }
    
    // 0x0C
    public int Unknown0C { get; init; }
}