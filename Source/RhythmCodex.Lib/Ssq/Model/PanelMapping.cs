using RhythmCodex.Infrastructure;

namespace RhythmCodex.Ssq.Model
{
    [Model]
    public class PanelMapping
    {
        public int Player { get; init; }
        public int Panel { get; init; }
    }
}