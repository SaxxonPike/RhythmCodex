using System;

namespace RhythmCodex.Archs.Djmain.Converters;

/// <summary>
/// Handles the conversion of Djmain's audio formats to float.
/// </summary>
public interface IDjmainAudioDecoder
{
    /// <summary>
    /// Decodes 4-bit DPCM audio data.
    /// </summary>
    float[] DecodeDpcm(ReadOnlySpan<byte> data);

    /// <summary>
    /// Decodes 8-bit PCM audio data.
    /// </summary>
    float[] DecodePcm8(ReadOnlySpan<byte> data);

    /// <summary>
    /// Decodes 16-bit PCM audio data.
    /// </summary>
    float[] DecodePcm16(ReadOnlySpan<byte> data);
}