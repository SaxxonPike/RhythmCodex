namespace RhythmCodex.Ssq.Model
{
    public class Step : IStep
    {
        public int MetricOffset { get; set; }
        public byte Panels { get; set; }
        public byte? ExtraPanels { get; set; }
        public byte? ExtraPanelInfo { get; set; }
    }
}
