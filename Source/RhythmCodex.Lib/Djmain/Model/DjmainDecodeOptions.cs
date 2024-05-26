using RhythmCodex.Infrastructure;

namespace RhythmCodex.Djmain.Model;

[Model]
public record DjmainDecodeOptions
{
    public bool DisableAudio { get; init; }
    public bool DoNotConsolidateSamples { get; init; }
}