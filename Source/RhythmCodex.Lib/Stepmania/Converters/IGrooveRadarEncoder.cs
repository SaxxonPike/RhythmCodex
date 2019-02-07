using RhythmCodex.Charting;
using RhythmCodex.Meta.Models;

namespace RhythmCodex.Stepmania.Converters
{
    public interface IGrooveRadarEncoder
    {
        string Encode(IMetadata metadata);
    }
}