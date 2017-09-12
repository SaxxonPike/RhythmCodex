using RhythmCodex.Infrastructure;

namespace RhythmCodex.Djmain
{
    [Service]
    public class DjmainConfiguration : IDjmainConfiguration
    {
        public int MaxSampleDefinitions { get; } = 256;
    }
}
