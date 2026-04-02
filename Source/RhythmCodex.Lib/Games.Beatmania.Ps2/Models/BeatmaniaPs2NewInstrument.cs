namespace RhythmCodex.Games.Beatmania.Ps2.Models;

public class BeatmaniaPs2NewInstrument
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
    public byte VolumeLeft { get; set; }
    
    // 0x09
    public byte VolumeRight { get; set; }
    
    // 0x0A
    public ushort SampleNumber { get; set; }
    
    // 0x0C
    public int Unknown0C { get; set; }
}