using System.Collections.Generic;
using RhythmCodex.Games.Mgs.Models;
using RhythmCodex.Sounds.Models;

namespace RhythmCodex.Games.Mgs.Converters;

/// <summary>
/// Renders Metal Gear Solid sound scripts into PCM audio.
/// </summary>
public interface IMgsSdSoundScriptRenderer
{
    /// <summary>
    /// Renders a sound script into PCM audio.
    /// </summary>
    /// <param name="script">
    /// Script to render.
    /// </param>
    /// <param name="soundBank">
    /// Sound bank that will be used as the audio data source.
    /// </param>
    /// <param name="sampleRate">
    /// Sampling rate for the output mix.
    /// </param>
    /// <returns>
    /// A stereo mix of all the sounds in the script.
    /// </returns>
    Sound Render(MgsSdSoundScript script, List<MgsSdSoundBankEntryWithData> soundBank, int sampleRate);
}