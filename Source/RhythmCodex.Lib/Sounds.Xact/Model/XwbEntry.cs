using RhythmCodex.Infrastructure;

namespace RhythmCodex.Sounds.Xact.Model;

[Model]
public struct XwbEntry
{
    public int Value { get; set; }
    public XwbMiniWaveFormat Format { get; set; }
    public XwbRegion PlayRegion { get; set; }
    public XwbSampleRegion LoopRegion { get; set; }
        
    public int Flags => Value & 0xF;
    public int Duration => Value >> 4;
}