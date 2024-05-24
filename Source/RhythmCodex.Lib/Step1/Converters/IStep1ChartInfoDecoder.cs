using RhythmCodex.Ssq.Model;

namespace RhythmCodex.Step1.Converters;

public interface IStep1ChartInfoDecoder
{
    ChartInfo Decode(int metadata, int playerCount, int panelCount);
}