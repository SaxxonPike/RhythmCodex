using RhythmCodex.IoC;

namespace RhythmCodex.Archs.Djmain;

[Service]
public class DjmainConfiguration : IDjmainConfiguration
{
    public int MaxSampleDefinitions { get; } = 256;
}