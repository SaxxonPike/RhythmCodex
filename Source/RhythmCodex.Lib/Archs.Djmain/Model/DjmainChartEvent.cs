using RhythmCodex.Infrastructure;

namespace RhythmCodex.Archs.Djmain.Model;

[Model]
public record DjmainChartEvent
{
    public ushort Offset { get; set; }

    public byte Param0 { get; set; }

    public byte Param1 { get; set; }
}