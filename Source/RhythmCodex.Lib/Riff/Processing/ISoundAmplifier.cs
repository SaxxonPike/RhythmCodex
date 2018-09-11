using RhythmCodex.Infrastructure.Models;

namespace RhythmCodex.Riff.Processing
{
    public interface ISoundAmplifier
    {
        void Amplify(ISound sound, float volume, float panning);
    }
}