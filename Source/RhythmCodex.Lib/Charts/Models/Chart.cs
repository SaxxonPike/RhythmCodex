using System.Collections.Generic;
using RhythmCodex.Infrastructure;
using RhythmCodex.Metadatas.Models;

namespace RhythmCodex.Charts.Models;

[Model]
public class Chart : Metadata
{
    public List<Event> Events { get; set; } = [];
}