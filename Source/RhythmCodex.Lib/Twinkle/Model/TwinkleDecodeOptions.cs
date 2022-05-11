using RhythmCodex.Infrastructure;

namespace RhythmCodex.Twinkle.Model;

[Model]
public class TwinkleDecodeOptions
{
    public bool DisableAudio { get; set; }
    public bool DoNotConsolidateSamples { get; set; }
}