using System.Collections.Generic;
using RhythmCodex.Charting.Models;
using RhythmCodex.Infrastructure;
using RhythmCodex.Sounds.Models;

namespace RhythmCodex.Djmain.Model;

public class DjmainArchive
{
    public int Id { get; set; }
    public List<Chart> Charts { get; set; } = [];
    public List<Sound> Samples { get; set; } = [];

    public Dictionary<int, List<DjmainChartEvent>> RawCharts { get; set; } = new();

    public override string ToString() => Json.Serialize(this);
}