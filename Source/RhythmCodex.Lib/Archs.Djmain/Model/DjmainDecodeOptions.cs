using RhythmCodex.Infrastructure;

namespace RhythmCodex.Archs.Djmain.Model;

[Model]
public class DjmainDecodeOptions
{
    public bool DisableAudio { get; init; }
    public bool DoNotConsolidateSamples { get; init; }
    public bool SwapStereo { get; init; } = true;
}