using System.Collections.Generic;
using System.Linq;
using RhythmCodex.Charts.Ssq.Model;
using RhythmCodex.Extensions;

namespace RhythmCodex.Charts.Ssq.Mappers;

public abstract class DdrPanelMapperBase : IPanelMapper
{
    public abstract int PanelCount { get; }
    public abstract int PlayerCount { get; }
    protected abstract Dictionary<int, PanelMapping> PanelMap { get; }

    public PanelMapping? Map(int panel) =>
        PanelMap.GetValueOrDefault(panel);

    public int? Map(PanelMapping mapping)
    {
        var mapped = PanelMap
            .SingleOrDefault(kv => kv.Value.Panel == mapping.Panel &&
                                   kv.Value.Player == mapping.Player);

        return mapped.Value == null ? null : mapped.Key;
    }

    public bool ShouldMap(IEnumerable<int> panels)
    {
        var requiredPanels = PanelMap.Keys;

        return panels
            .Distinct()
            .Union(requiredPanels)
            .Count() == requiredPanels.Count;
    }

    public bool ShouldMap(IEnumerable<PanelMapping> panels)
    {
        var requiredPanelMappings = PanelMap.Values;
        var inPanels = panels.AsCollection();

        return requiredPanelMappings.All(InputMatchesMapping) &&
               inPanels.All(MappingMatchesInput);

        bool MappingMatchesInput(PanelMapping p) =>
            requiredPanelMappings.Any(pm => p.Panel == pm.Panel && p.Player == pm.Player);

        bool InputMatchesMapping(PanelMapping pm) =>
            inPanels.Any(p => p.Panel == pm.Panel && p.Player == pm.Player);
    }
}