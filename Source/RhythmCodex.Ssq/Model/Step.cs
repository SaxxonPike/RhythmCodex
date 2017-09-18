using RhythmCodex.Infrastructure;

namespace RhythmCodex.Ssq.Model
{
    [Model]
    public class Step
    {
        public int MetricOffset { get; set; }
        public byte Panels { get; set; }
        public byte? ExtraPanels { get; set; }
        public byte? ExtraPanelInfo { get; set; }
    }
}
