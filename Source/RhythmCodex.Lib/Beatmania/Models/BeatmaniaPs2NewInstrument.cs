namespace RhythmCodex.Beatmania.Models;

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

public class BeatmaniaPs2NewSampleHeader
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

public class BeatmaniaPs2NewSample
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
    public byte FineFreq { get; set; }
    
    // 0x0C
    public int Unknown0C { get; set; }
}