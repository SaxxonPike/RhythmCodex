using RhythmCodex.Ssq.Model;

namespace RhythmCodex.Ssq.Converters
{
    public interface IPanelMapper
    {
        int PanelCount { get; }
        int PlayerCount { get; }
        PanelMapping Map(int panel);
        int? Map(PanelMapping mapping);
    }
}