namespace RhythmCodex.Games.Beatmania.Ps2.Models;

public readonly record struct BeatmaniaPs2NewInstrument
{
    public int Index { get; init; }
    
    // 0x00
    public byte Flags00 { get; init; }
    
    // 0x01
    public byte PlaybackChannel { get; init; }
    
    // 0x02
    public byte Flags02 { get; init; }
    
    // 0x03
    public byte SampleChannelCount { get; init; }
    
    // 0x04
    public int Unknown04 { get; init; }
    
    // 0x08
    public byte SampleChannel0Pan { get; init; }
    
    // 0x09
    public byte SampleChannel1Pan { get; init; }
    
    // 0x0A
    public ushort SampleNumber { get; init; }
    
    // 0x0C
    public byte Volume { get; init; }
    
    // 0x0D
    public byte Unknown0D { get; init; }
    
    // 0x0E
    public ushort Unknown0E { get; init; }
}