namespace RhythmCodex.Games.Beatmania.Ps2.Models;

public record BeatmaniaPs2NewSample
{
    public int Index { get; set; }
    
    // 0x00
    public int SampleOffset { get; set; }
    
    // 0x04
    public int SampleLength { get; set; }
    
    // 0x08
    public byte ChannelCount { get; set; }
    
    // 0x09
    public byte Unknown09 { get; set; }
    
    // 0x0A
    public byte CoarseFreq { get; set; }
    
    // 0x0B
    public sbyte FineFreq { get; set; }
    
    // 0x0C
    public int Unknown0C { get; set; }
}