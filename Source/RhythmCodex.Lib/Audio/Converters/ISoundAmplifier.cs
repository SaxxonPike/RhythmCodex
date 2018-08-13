namespace RhythmCodex.Audio.Converters
{
    public interface ISoundAmplifier
    {
        void Amplify(ISound sound, float volume, float panning);
    }
}