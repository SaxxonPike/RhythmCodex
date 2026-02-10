using RhythmCodex.Infrastructure;

namespace RhythmCodex.Sounds.Wav.Models;

[Model]
public class ChartRendererOptions
{
    public BigRational SampleRate { get; set; } = 44100;
    public bool SwapStereo { get; set; }
    public bool UseSourceDataForSamples { get; set; }
}