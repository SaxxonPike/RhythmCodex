using RhythmCodex.Charts.Ssq.Model;

namespace RhythmCodex.Charts.Step1.Converters;

public interface IStep1ChartInfoDecoder
{
    ChartInfo Decode(int metadata, int playerCount, int panelCount);
}