using RhythmCodex.Charts.Models;

namespace RhythmCodex.Charts.Ssq.Converters
{
    public interface ISsqIdSelector
    {
        int? SelectDifficulty(Chart metadata);
        int? SelectType(Chart metadata);
    }
}