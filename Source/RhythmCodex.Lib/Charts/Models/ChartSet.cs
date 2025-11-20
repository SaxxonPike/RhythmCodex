using System.Collections.Generic;
using RhythmCodex.Infrastructure;
using RhythmCodex.Metadatas.Models;

namespace RhythmCodex.Charts.Models;

[Model]
public class ChartSet
{
    public IMetadata? Metadata { get; set; }
    public List<Chart> Charts { get; set; } = [];
}