using System;
using RhythmCodex.Sounds.Mixer.Models;
using RhythmCodex.Sounds.Models;

namespace RhythmCodex.Sounds.Mixer.Converters;

/// <summary>
/// Handles mixing down of a <see cref="Sound"/> into a larger buffer.
/// </summary>
public interface IStereoMixer
{
    /// <summary>
    /// Performs a stereo mixdown of sound data.
    /// </summary>
    /// <param name="outLeft">
    /// Buffer to mix the left channel into.
    /// </param>
    /// <param name="outRight">
    /// Buffer to mix the right channel into.
    /// </param>
    /// <param name="state">
    /// Mix state of the sound.
    /// </param>
    /// <returns>
    /// Updated mix state. Sample offset will reflect the amount of data actually written.
    /// If the sound has finished playing, or has no sample data, this returns null.
    /// </returns>
    /// <remarks>
    /// The sampling rate of the input and output must match. This is not checked,
    /// and doing so anyway will produce unexpected results.
    /// </remarks>
    (MixState State, int Mixed) Mix(Span<float> outLeft,
        Span<float> outRight,
        MixState state);
}