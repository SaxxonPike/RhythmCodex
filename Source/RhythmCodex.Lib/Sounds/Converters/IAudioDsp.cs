using RhythmCodex.Infrastructure;
using RhythmCodex.Sounds.Models;
using RhythmCodex.Sounds.Providers;

namespace RhythmCodex.Sounds.Converters;

public interface IAudioDsp
{
    Sound? ApplyEffects(Sound? sound);
    Sound? ApplyPanVolume(Sound sound, BigRational volume, BigRational panning);
    Sound? ApplyResampling(Sound? sound, IResampler resampler, BigRational rate);
    Sound? Normalize(Sound sound, BigRational target, bool cutOnly);
    Sound IntegerDownsample(Sound sound, int factor);
}