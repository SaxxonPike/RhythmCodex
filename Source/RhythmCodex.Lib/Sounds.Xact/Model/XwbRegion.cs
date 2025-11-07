using RhythmCodex.Infrastructure;

namespace RhythmCodex.Sounds.Xact.Model;

[Model]
public struct XwbRegion
{
    public int Offset { get; set; }
    public int Length { get; set; }
}