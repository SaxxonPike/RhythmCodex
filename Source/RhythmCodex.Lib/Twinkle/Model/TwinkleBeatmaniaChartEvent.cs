using RhythmCodex.Infrastructure;

namespace RhythmCodex.Twinkle.Model;

[Model]
public class TwinkleBeatmaniaChartEvent
{
    public ushort Offset { get; set; }
    public byte Param { get; set; }
    public byte Value { get; set; }
}