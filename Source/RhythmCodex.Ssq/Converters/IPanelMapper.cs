using RhythmCodex.Ssq.Model;

namespace RhythmCodex.Ssq.Converters
{
    public interface IPanelMapper
    {
        PanelMapping Map(int panel);
        int? Map(PanelMapping mapping);
        int PanelCount { get; }
        int PlayerCount { get; }
    }
}