using RhythmCodex.Infrastructure;

namespace RhythmCodex.Djmain.Model;

[Model]
public record DjmainChartEvent
{
    public ushort Offset { get; init; }

    public byte Param0 { get; init; }

    public byte Param1 { get; init; }

    public override string ToString() => Json.Serialize(this);
}