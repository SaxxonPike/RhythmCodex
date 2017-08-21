using System.Numerics;
using Numerics;

namespace RhythmCodex.Djmain
{
    public class DjmainConfiguration : IDjmainConfiguration
    {
        public static readonly DjmainConfiguration Default = new DjmainConfiguration();

        public int ChunkSize { get; } = 0x1000000;
        public int MaxSampleDefinitions { get; } = 256;
        public int DpcmEndMarker { get; } = 0x44444444 << 1;
        public long Pcm8EndMarker { get; } = 0x4040404040404040 << 1;
        public long Pcm16EndMarker { get; } = 0x4000400040004000 << 1;
        public BigRational SampleRateMultiplier { get; } = new BigRational(44100, 60216);
    }
}
