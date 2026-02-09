using RhythmCodex.Infrastructure;

namespace RhythmCodex.Games.Beatmania.Converters;

public interface IBeatmaniaDspTranslator
{
    BigRational GetLinearVolume(int volume);
    BigRational GetDjmainVolume(int volume);
    BigRational GetTwinkleVolume(int volume);
    BigRational GetBm2dxPanning(int panning);
    BigRational GetDjmainPanning(int panning, bool swap);
    BigRational GetDjmainRate(int rate);
    BigRational GetFirebeatVolume(int volume);
}