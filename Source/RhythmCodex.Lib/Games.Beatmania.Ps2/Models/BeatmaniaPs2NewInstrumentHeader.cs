namespace RhythmCodex.Games.Beatmania.Ps2.Models;

public class BeatmaniaPs2NewInstrumentHeader
{
    // 0x00
    public int Identifier { get; set; }
    
    // 0x04
    public int BlockCount { get; set; }
    
    // 0x08
    public byte VolumeLeft { get; set; }
    
    // 0x09
    public byte VolumeRight { get; set; }
    
    // 0x0A
    public ushort Unknown0A { get; set; }
    
    // 0x0C
    public int Unknown0C { get; set; }
}