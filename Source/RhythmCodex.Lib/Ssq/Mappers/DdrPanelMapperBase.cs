using System.Collections.Generic;
using System.Linq;
using RhythmCodex.Extensions;
using RhythmCodex.Ssq.Model;

namespace RhythmCodex.Ssq.Mappers
{
    public abstract class DdrPanelMapperBase : IPanelMapper
    {
        public abstract int PanelCount { get; }
        public abstract int PlayerCount { get; }
        protected abstract IDictionary<int, IPanelMapping> PanelMap { get; }

        public IPanelMapping Map(int panel)
        {
            if (!PanelMap.ContainsKey(panel))
                return null;
            return PanelMap[panel];
        }

        public int? Map(IPanelMapping mapping)
        {
            var mapped = PanelMap
                .SingleOrDefault(kv => kv.Value.Panel == mapping.Panel && kv.Value.Player == mapping.Player);
            return mapped.Value == null ? (int?)null : mapped.Key;
        }

        public bool ShouldMap(IEnumerable<int> panels)
        {
            var requiredPanels = PanelMap.Keys;
            return panels
                       .Distinct()
                       .Union(requiredPanels)
                       .Count() == requiredPanels.Count;
        }

        public bool ShouldMap(IEnumerable<IPanelMapping> panels)
        {
            var requiredPanelMappings = PanelMap.Values;
            var inPanels = panels.AsList();
            
            return requiredPanelMappings.All(pm => inPanels.Any(p => p.Panel == pm.Panel && p.Player == pm.Player))
                   && inPanels.All(p => requiredPanelMappings.Any(pm => p.Panel == pm.Panel && p.Player == pm.Player));
        }
    }
}