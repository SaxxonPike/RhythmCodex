namespace RhythmCodex.Games.Beatmania.Ps2.Models;

public readonly record struct BeatmaniaPs2NewInstrumentHeader
{
    // 0x00
    public int Identifier { get; init; }
    
    // 0x04
    public int BlockCount { get; init; }
    
    // 0x08
    public byte Volume { get; init; }
    
    // 0x09
    public byte Unknown09 { get; init; }
    
    // 0x0A
    public ushort Unknown0A { get; init; }
    
    // 0x0C
    public int Unknown0C { get; init; }
}