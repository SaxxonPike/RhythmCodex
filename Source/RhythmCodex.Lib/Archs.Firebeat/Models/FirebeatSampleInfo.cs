using RhythmCodex.Infrastructure;

namespace RhythmCodex.Archs.Firebeat.Models;

[Model]
public class FirebeatSampleInfo
{
    public byte Channel { get; set; }
    public byte Flag01 { get; set; }
    public ushort Frequency { get; set; }
    public byte Volume { get; set; }
    public byte Panning { get; set; }
    public int SampleOffset { get; set; }
    public int SampleLength { get; set; }
    public ushort Value0C { get; set; }
    public byte Flag0E { get; set; }
    public FirebeatSampleFlag0F Flag0F { get; set; }
    public ushort SizeInBlocks { get; set; }
}