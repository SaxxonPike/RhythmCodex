using Numerics;

namespace RhythmCodex.Djmain
{
    public interface IDjmainConfiguration
    {
        int ChunkSize { get; }
        int MaxSampleDefinitions { get; }
        BigRational SampleRateMultiplier { get; }
    }
}