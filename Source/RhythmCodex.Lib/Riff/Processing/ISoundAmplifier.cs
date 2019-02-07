using RhythmCodex.Infrastructure.Models;
using RhythmCodex.Sounds.Models;

namespace RhythmCodex.Riff.Processing
{
    public interface ISoundAmplifier
    {
        void Amplify(ISound sound, float volume, float panning);
    }
}