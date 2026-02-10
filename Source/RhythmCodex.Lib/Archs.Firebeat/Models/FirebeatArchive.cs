using System.Collections.Generic;
using RhythmCodex.Charts.Models;
using RhythmCodex.Infrastructure;
using RhythmCodex.Sounds.Models;

namespace RhythmCodex.Archs.Firebeat.Models;

public class FirebeatArchive
{
    public int Id { get; set; }

    public List<Chart> Charts { get; set; } = [];

    public List<Sound> Samples { get; set; } = [];

    public Dictionary<int, List<FirebeatChartEvent>> RawCharts { get; set; } = new();

    public Dictionary<int, FirebeatSampleInfo> RawSampleInfos { get; set; } = new();

    public override string ToString() => Json.Serialize(this);
}