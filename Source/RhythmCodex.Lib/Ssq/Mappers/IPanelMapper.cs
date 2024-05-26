using System.Collections.Generic;
using RhythmCodex.Ssq.Model;

namespace RhythmCodex.Ssq.Mappers;

public interface IPanelMapper
{
    int PanelCount { get; }
    int PlayerCount { get; }
    IPanelMapping? Map(int panel);
    int? Map(IPanelMapping mapping);

    bool ShouldMap(IEnumerable<int> panels);
    bool ShouldMap(IEnumerable<IPanelMapping> panels);
}