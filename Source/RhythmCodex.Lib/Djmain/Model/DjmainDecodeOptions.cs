using RhythmCodex.Infrastructure;

namespace RhythmCodex.Djmain.Model;

[Model]
public class DjmainDecodeOptions
{
    public bool DisableAudio { get; init; }
    public bool DoNotConsolidateSamples { get; init; }
}