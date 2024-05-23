using RhythmCodex.Infrastructure;

namespace RhythmCodex.Ssq.Model;

[Model]
public class PanelMapping : IPanelMapping
{
    public int Player { get; set; }
    public int Panel { get; set; }
}