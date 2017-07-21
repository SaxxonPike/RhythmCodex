namespace RhythmCodex.Ssq.Model
{
    public class Step : IStep
    {
        public int MetricOffset { get; set; }
        public int Panels { get; set; }
        public int? ExtraPanels { get; set; }
    }
}
