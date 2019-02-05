using RhythmCodex.Infrastructure;

namespace RhythmCodex.Beatmania.Converters
{
    public interface IBeatmaniaDspTranslator
    {
        BigRational GetLinearVolume(int volume);
        BigRational GetDjmainVolume(int volume);
        BigRational GetBm2dxPanning(int panning);
        BigRational GetDjmainPanning(int panning);
        BigRational GetDjmainRate(int rate);
        BigRational GetFirebeatVolume(int volume);
    }
}