using RhythmCodex.Infrastructure.Models;

namespace RhythmCodex.Dsp
{
    public interface IAudioDsp
    {
        ISound ApplyEffects(ISound sound);
    }
}