using System.Collections.Generic;
using RhythmCodex.Infrastructure;
using RhythmCodex.Ssq.Converters;
using RhythmCodex.Ssq.Model;

namespace RhythmCodex.Ssq.Mappers
{
    [Service]
    public class Ddr1Player3PanelMapper : DdrPanelMapperBase
    {
        public sealed override int PanelCount => 3;
        public sealed override int PlayerCount => 1;

        protected override IDictionary<int, IPanelMapping> PanelMap => new Dictionary<int, IPanelMapping>
        {
            {1, new PanelMapping {Player = 0, Panel = 2}},
            {4, new PanelMapping {Player = 0, Panel = 1}},
            {6, new PanelMapping {Player = 0, Panel = 4}},
        };
    }
}