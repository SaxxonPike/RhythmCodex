using System.Collections.Generic;
using RhythmCodex.Charts.Models;
using RhythmCodex.Infrastructure;
using RhythmCodex.Sounds.Models;

namespace RhythmCodex.Archs.Djmain.Model;

public class DjmainArchive
{
    public int Id { get; set; }

    public List<Chart> Charts { get; set; } = [];

    public List<Sound> Samples { get; set; } = [];

    public Dictionary<int, List<DjmainChartEvent>> RawCharts { get; set; } = new();

    public Dictionary<int, Dictionary<int, DjmainSampleInfo>> SampleInfos { get; set; } = new();

    public override string ToString() => Json.Serialize(this);
}