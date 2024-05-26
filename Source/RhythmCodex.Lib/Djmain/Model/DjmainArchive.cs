using System.Collections.Generic;
using RhythmCodex.Charting.Models;
using RhythmCodex.Infrastructure;
using RhythmCodex.Sounds.Models;

namespace RhythmCodex.Djmain.Model;

public record DjmainArchive
{
    public int Id { get; init; }

    public List<Chart> Charts { get; init; } = [];

    public List<Sound> Samples { get; init; } = [];

    public Dictionary<int, List<DjmainChartEvent>> RawCharts { get; init; } = new();

    public override string ToString() => Json.Serialize(this);
}