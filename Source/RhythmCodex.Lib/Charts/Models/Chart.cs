using System.Collections.Generic;
using System.Linq;
using RhythmCodex.Infrastructure;
using RhythmCodex.Metadatas.Models;

namespace RhythmCodex.Charts.Models;

[Model]
public class Chart : Metadata
{
    public List<Event> Events { get; set; } = [];

    public Chart Clone()
    {
        var result = new Chart
        {
            Events = new List<Event>(Events.Select(e => e.Clone()))
        };

        result.CloneMetadataFrom(this);
        return result;
    }
}