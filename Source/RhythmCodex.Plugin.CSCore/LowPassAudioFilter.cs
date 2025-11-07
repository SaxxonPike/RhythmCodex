using RhythmCodex.IoC;
using RhythmCodex.Plugin.CSCore.Lib.DSP;
using RhythmCodex.Sounds.Filter.Providers;

namespace RhythmCodex.Plugin.CSCore;

[Service]
public class LowPassAudioFilter : IFilter
{
    public string Name => "lowpass";
    public int Priority => 0;
    public FilterType Type => FilterType.LowPass;
        
    public IFilterContext Create(double sampleRate, double cutoff)
    {
        return new LowpassFilter(sampleRate, cutoff);
    }
}