using RhythmCodex.Infrastructure;

namespace RhythmCodex.Bms.Model;

[Model]
public class BmsEvent
{
    public string? Lane { get; set; }
    public int Measure { get; set; }
    public BigRational Offset { get; set; }
    public BigRational Value { get; set; }
}