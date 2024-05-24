using RhythmCodex.Infrastructure;

namespace RhythmCodex.Djmain.Model;

[Model]
public class DjmainDecodeOptions
{
    public bool DisableAudio { get; set; }
    public bool DoNotConsolidateSamples { get; set; }
}