using System.Collections.Generic;
using RhythmCodex.Charts.Ssq.Model;
using RhythmCodex.IoC;

namespace RhythmCodex.Charts.Ssq.Mappers;

[Service]
public class Ddr1Player3PanelMapper : DdrPanelMapperBase
{
    public sealed override int PanelCount => 3;
    public sealed override int PlayerCount => 1;

    protected override Dictionary<int, PanelMapping> PanelMap => new()
    {
        {1, new PanelMapping {Player = 0, Panel = 1}},
        {4, new PanelMapping {Player = 0, Panel = 0}},
        {6, new PanelMapping {Player = 0, Panel = 2}}
    };
}