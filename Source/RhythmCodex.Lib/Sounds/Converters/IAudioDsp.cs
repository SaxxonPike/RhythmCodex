using RhythmCodex.Infrastructure;
using RhythmCodex.Sounds.Models;

namespace RhythmCodex.Dsp
{
    public interface IAudioDsp
    {
        ISound ApplyEffects(ISound sound);
        ISound ApplyPanVolume(ISound sound, BigRational volume, BigRational panning);
        ISound ApplyResampling(ISound sound, BigRational rate);
    }
}