using System;
using System.Collections.Generic;
using RhythmCodex.Infrastructure;
using RhythmCodex.Metadatas.Models;
using RhythmCodex.Sounds.Models;
using RhythmCodex.Sounds.Resampler.Providers;

namespace RhythmCodex.Sounds.Converters;

/// <summary>
/// Handles processing of raw audio data.
/// </summary>
public interface IAudioDsp
{
    /// <summary>
    /// Applies gain and panning effects.
    /// </summary>
    Sound ApplyEffects(Sound sound, Metadata? mixerMetadata = null);
    
    /// <summary>
    /// Resamples audio data to a new sampling rate using the specified resampler.
    /// </summary>
    Sound ApplyResampling(Sound sound, IResampler resampler, BigRational rate);
    
    /// <summary>
    /// Normalizes the audio data such that the peak is at the specified target.
    /// </summary>
    Sound? Normalize(Sound sound, BigRational target, bool cutOnly);
    
    Sound IntegerDownsample(Sound sound, int factor);
    
    /// <summary>
    /// Mixes audio data into a new sound, applying gain and panning effects as necessary before mixing. Resampling
    /// is not performed - the input should share the same sampling rate.
    /// </summary>
    Sound Mix(IEnumerable<Sound> sound, Metadata? mixerMetadata = null);

    /// <summary>
    /// Mixes down and interleaves sound's sample data as 16-bit values.
    /// </summary>
    byte[] Interleave16Bits(Sound sound);
    
    /// <summary>
    /// Deinterleaves and converts raw sample data.
    /// </summary>
    /// <param name="data">
    /// Data to convert.
    /// </param>
    /// <param name="channels">
    /// Number of audio channels to deinterleave.
    /// </param>
    Sample[] FloatsToSamples(ReadOnlySpan<float> data, int channels);
}