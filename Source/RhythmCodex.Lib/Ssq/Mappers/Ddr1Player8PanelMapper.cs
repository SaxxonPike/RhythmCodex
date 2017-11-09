using System.Collections.Generic;
using RhythmCodex.Infrastructure;
using RhythmCodex.Ssq.Model;

namespace RhythmCodex.Ssq.Mappers
{
    [Service]
    public class Ddr1Player8PanelMapper : DdrPanelMapperBase
    {
        public sealed override int PanelCount => 8;
        public sealed override int PlayerCount => 1;

        protected override IDictionary<int, IPanelMapping> PanelMap => new Dictionary<int, IPanelMapping>
        {
            {0, new PanelMapping {Player = 0, Panel = 0}},
            {1, new PanelMapping {Player = 0, Panel = 1}},
            {2, new PanelMapping {Player = 0, Panel = 2}},
            {3, new PanelMapping {Player = 0, Panel = 3}},
            {4, new PanelMapping {Player = 0, Panel = 4}},
            {5, new PanelMapping {Player = 0, Panel = 5}},
            {6, new PanelMapping {Player = 0, Panel = 6}},
            {7, new PanelMapping {Player = 0, Panel = 7}}
        };
    }
}