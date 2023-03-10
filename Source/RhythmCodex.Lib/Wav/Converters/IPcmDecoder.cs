using System;

namespace RhythmCodex.Wav.Converters
{
    public interface IPcmDecoder
    {
        /// <summary>
        /// Decode unsigned 8-bit samples.
        /// </summary>
        float[] Decode8Bit(ReadOnlySpan<byte> bytes);
        
        /// <summary>
        /// Decode signed 16-bit samples.
        /// </summary>
        float[] Decode16Bit(ReadOnlySpan<byte> bytes);
        
        /// <summary>
        /// Decode signed 24-bit samples.
        /// </summary>
        float[] Decode24Bit(ReadOnlySpan<byte> bytes);
        
        /// <summary>
        /// Decode signed 32-bit integer samples.
        /// </summary>
        float[] Decode32Bit(ReadOnlySpan<byte> bytes);
        
        /// <summary>
        /// Decode 32-bit float samples.
        /// </summary>
        float[] DecodeFloat(ReadOnlySpan<byte> bytes);
    }
}