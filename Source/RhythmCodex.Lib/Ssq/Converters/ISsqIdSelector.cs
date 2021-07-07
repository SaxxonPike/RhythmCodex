using RhythmCodex.Charting.Models;

namespace RhythmCodex.Ssq.Converters
{
    public interface ISsqIdSelector
    {
        int? SelectDifficulty(Chart metadata);
        int? SelectType(Chart metadata);
    }
}