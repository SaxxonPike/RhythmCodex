using RhythmCodex.Infrastructure;
using RhythmCodex.Sounds.Models;
using RhythmCodex.Sounds.Providers;

namespace RhythmCodex.Sounds.Converters;

public interface IAudioDsp
{
    ISound ApplyEffects(ISound sound);
    ISound ApplyPanVolume(ISound sound, BigRational volume, BigRational panning);
    ISound ApplyResampling(ISound sound, IResampler resampler, BigRational rate);
    ISound Normalize(ISound sound, BigRational target, bool cutOnly);
    ISound IntegerDownsample(ISound sound, int factor);
}