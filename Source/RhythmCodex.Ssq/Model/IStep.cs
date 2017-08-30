namespace RhythmCodex.Ssq.Model
{
    public interface IStep
    {
        byte? ExtraPanelInfo { get; }
        byte? ExtraPanels { get; }
        int MetricOffset { get; }
        byte Panels { get; }
    }
}