using RhythmCodex.IoC;

namespace RhythmCodex.Djmain
{
    [Service]
    public class DjmainConfiguration : IDjmainConfiguration
    {
        public int MaxSampleDefinitions { get; } = 256;
    }
}