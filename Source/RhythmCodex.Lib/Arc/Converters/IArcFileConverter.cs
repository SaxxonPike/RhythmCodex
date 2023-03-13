using RhythmCodex.Arc.Model;

namespace RhythmCodex.Arc.Converters;

public interface IArcFileConverter
{
    ArcFile Compress(ArcFile file);
    ArcFile Decompress(ArcFile file);
}