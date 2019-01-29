﻿using System;
using System.Collections.Generic;
using System.Linq;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;
using RhythmCodex.Ssq.Model;

namespace RhythmCodex.Ssq.Mappers
{
    [Service]
    public class Ddr2Player4PanelMapper : DdrPanelMapperBase
    {
        public sealed override int PanelCount => 4;
        public sealed override int PlayerCount => 2;

        protected override IDictionary<int, IPanelMapping> PanelMap => new Dictionary<int, IPanelMapping>
        {
            {0, new PanelMapping {Player = 0, Panel = 0}},
            {1, new PanelMapping {Player = 0, Panel = 1}},
            {2, new PanelMapping {Player = 0, Panel = 2}},
            {3, new PanelMapping {Player = 0, Panel = 3}},
            {4, new PanelMapping {Player = 1, Panel = 0}},
            {5, new PanelMapping {Player = 1, Panel = 1}},
            {6, new PanelMapping {Player = 1, Panel = 2}},
            {7, new PanelMapping {Player = 1, Panel = 3}}
        };

    }
}