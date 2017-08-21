using Numerics;

namespace RhythmCodex.Djmain
{
    public interface IDjmainConfiguration
    {
        int ChunkSize { get; }
        int DpcmEndMarker { get; }
        int MaxSampleDefinitions { get; }
        long Pcm16EndMarker { get; }
        long Pcm8EndMarker { get; }
        BigRational SampleRateMultiplier { get; }
    }
}