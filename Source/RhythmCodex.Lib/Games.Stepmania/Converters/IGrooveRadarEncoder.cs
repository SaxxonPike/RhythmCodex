using RhythmCodex.Metadatas.Models;

namespace RhythmCodex.Games.Stepmania.Converters;

public interface IGrooveRadarEncoder
{
    string Encode(IMetadata metadata);
}