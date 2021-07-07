using RhythmCodex.Infrastructure;

namespace RhythmCodex.Ssq.Model
{
    [Model]
    public class Step
    {
        public int MetricOffset { get; init; }
        public byte Panels { get; set; }
        public byte? ExtraPanels { get; init; }
        public byte? ExtraPanelInfo { get; init; }
    }
}