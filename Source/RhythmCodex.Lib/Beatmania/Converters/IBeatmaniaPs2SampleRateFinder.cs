using RhythmCodex.Infrastructure;

namespace RhythmCodex.Beatmania.Converters
{
    public interface IBeatmaniaPs2SampleRateFinder
    {
        BigRational GetRate(int encodedRate);
    }
}