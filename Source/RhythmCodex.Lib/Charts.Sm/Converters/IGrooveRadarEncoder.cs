using RhythmCodex.Metadatas.Models;

namespace RhythmCodex.Charts.Sm.Converters;

public interface IGrooveRadarEncoder
{
    string Encode(IMetadata metadata);
}