using RhythmCodex.Infrastructure;
using RhythmCodex.Sounds.Models;

namespace RhythmCodex.Sounds.Converters
{
    public interface IAudioDsp
    {
        ISound ApplyEffects(ISound sound);
        ISound ApplyPanVolume(ISound sound, BigRational volume, BigRational panning);
        ISound ApplyResampling(ISound sound, BigRational rate);
        ISound Normalize(ISound sound, BigRational target);
    }
}