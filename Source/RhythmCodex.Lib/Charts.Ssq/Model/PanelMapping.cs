using RhythmCodex.Infrastructure;

namespace RhythmCodex.Charts.Ssq.Model;

[Model]
public record PanelMapping
{
    public int Player { get; set; }
    public int Panel { get; set; }
}