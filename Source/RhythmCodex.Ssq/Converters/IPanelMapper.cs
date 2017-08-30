using RhythmCodex.Ssq.Model;

namespace RhythmCodex.Ssq.Converters
{
    public interface IPanelMapper
    {
        IPanelMapping Map(int panel);
        int? Map(IPanelMapping mapping);
    }
}