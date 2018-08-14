namespace RhythmCodex.Audio.Processing
{
    public interface ISoundAmplifier
    {
        void Amplify(ISound sound, float volume, float panning);
    }
}