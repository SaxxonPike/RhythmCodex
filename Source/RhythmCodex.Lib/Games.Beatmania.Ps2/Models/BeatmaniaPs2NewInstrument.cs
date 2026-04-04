namespace RhythmCodex.Games.Beatmania.Ps2.Models;

public record BeatmaniaPs2NewInstrument
{
    public int Index { get; set; }
    
    // 0x00
    public byte Flags00 { get; set; }
    
    // 0x01
    public byte PlaybackChannel { get; set; }
    
    // 0x02
    public byte Flags02 { get; set; }
    
    // 0x03
    public byte SampleChannelCount { get; set; }
    
    // 0x04
    public int Unknown04 { get; set; }
    
    // 0x08
    public byte SampleChannel0Pan { get; set; }
    
    // 0x09
    public byte SampleChannel1Pan { get; set; }
    
    // 0x0A
    public ushort SampleNumber { get; set; }
    
    // 0x0C
    public byte Volume { get; set; }
    
    // 0x0D
    public byte Unknown0D { get; set; }
    
    // 0x0E
    public ushort Unknown0E { get; set; }
}