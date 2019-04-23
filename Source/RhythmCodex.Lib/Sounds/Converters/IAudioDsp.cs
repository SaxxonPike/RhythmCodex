using RhythmCodex.Infrastructure;
using RhythmCodex.Infrastructure.Models;
using RhythmCodex.Sounds.Models;

namespace RhythmCodex.Dsp
{
    public interface IAudioDsp
    {
        ISound ApplyEffects(ISound sound);
        ISound ApplyResampling(ISound sound, BigRational rate);
    }
}