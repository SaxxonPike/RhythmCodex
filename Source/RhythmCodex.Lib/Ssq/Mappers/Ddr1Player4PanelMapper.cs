using System.Collections.Generic;
using RhythmCodex.IoC;
using RhythmCodex.Ssq.Model;

namespace RhythmCodex.Ssq.Mappers;

[Service]
public class Ddr1Player4PanelMapper : DdrPanelMapperBase
{
    public sealed override int PanelCount => 4;
    public sealed override int PlayerCount => 1;

    protected override Dictionary<int, IPanelMapping> PanelMap => new()
    {
        {0, new PanelMapping {Player = 0, Panel = 0}},
        {1, new PanelMapping {Player = 0, Panel = 1}},
        {2, new PanelMapping {Player = 0, Panel = 2}},
        {3, new PanelMapping {Player = 0, Panel = 3}}
    };
}