using RhythmCodex.Infrastructure;

namespace RhythmCodex.Sounds.Xact.Model;

[Model]
public struct XwbSampleRegion
{
    public int StartSample { get; set; }
    public int TotalSamples { get; set; }
}