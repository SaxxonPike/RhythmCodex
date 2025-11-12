using RhythmCodex.Infrastructure;

namespace RhythmCodex.Sounds.Wav.Models;

[Model]
public class ChartRendererOptions
{
    public BigRational SampleRate { get; set; } = 44100;
    public BigRational? Volume { get; set; }
    public BigRational? BgmVolume { get; set; }
    public BigRational? KeyVolume { get; set; }
    public bool SwapStereo { get; set; }
    public bool UseSourceDataForSamples { get; set; }
    public bool LinearPanning { get; set; }
}