using RhythmCodex.Infrastructure;
using RhythmCodex.Metadatas.Models;

namespace RhythmCodex.Charts.Models;

[Model]
public class Event : Metadata
{
    public Event Clone()
    {
        var result = new Event();
        result.CloneMetadataFrom(this);
        return result;
    }
}