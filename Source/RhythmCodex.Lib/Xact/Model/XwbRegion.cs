using RhythmCodex.Infrastructure;

namespace RhythmCodex.Xact.Model;

[Model]
public struct XwbRegion
{
    public int Offset { get; set; }
    public int Length { get; set; }
}