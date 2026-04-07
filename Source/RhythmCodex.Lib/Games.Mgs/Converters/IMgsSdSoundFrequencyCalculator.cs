namespace RhythmCodex.Games.Mgs.Converters;

/// <summary>
/// Handles calculating the playback frequency of an MGS sound system sample.
/// </summary>
public interface IMgsSdSoundFrequencyCalculator
{
    /// <summary>
    /// Calculates the playback frequency (in hz) of the specified note.
    /// </summary>
    /// <param name="note">
    /// Base note in semitones.
    /// </param>
    /// <param name="macro">
    /// Macro tuning in semitones. Each octave is 12 semitones.
    /// </param>
    /// <param name="micro">
    /// Micro tuning in 1/128ths of semitones. Values -127 to +127.
    /// </param>
    /// <returns>
    /// Playback frequency in hz.
    /// </returns>
    float Calculate(int note, int macro, int micro);
}