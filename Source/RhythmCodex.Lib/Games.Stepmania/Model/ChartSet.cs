using System.Collections.Generic;
using RhythmCodex.Charts.Models;
using RhythmCodex.Infrastructure;
using RhythmCodex.Metadatas.Models;

namespace RhythmCodex.Games.Stepmania.Model;

[Model]
public class ChartSet
{
    public IMetadata? Metadata { get; set; }
    public List<Chart> Charts { get; set; } = [];
}