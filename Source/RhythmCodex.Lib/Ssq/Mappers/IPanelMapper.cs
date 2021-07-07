using System.Collections.Generic;
using RhythmCodex.Ssq.Model;

namespace RhythmCodex.Ssq.Mappers
{
    public interface IPanelMapper
    {
        int PanelCount { get; }
        int PlayerCount { get; }
        PanelMapping Map(int panel);
        int? Map(PanelMapping mapping);

        bool ShouldMap(IEnumerable<int> panels);
        bool ShouldMap(IEnumerable<PanelMapping> panels);
    }
}