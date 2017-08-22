using Numerics;

namespace RhythmCodex.Djmain
{
    public class DjmainConfiguration : IDjmainConfiguration
    {
        public static readonly DjmainConfiguration Default = new DjmainConfiguration();

        public int ChunkSize { get; } = 0x1000000;
        public int MaxSampleDefinitions { get; } = 256;
        public BigRational SampleRateMultiplier { get; } = new BigRational(44100, 60216);
    }
}
