namespace RhythmCodex.Ssq.Model
{
    public interface IStep
    {
        int? ExtraPanels { get; }
        int MetricOffset { get; }
        int Panels { get; }
    }
}