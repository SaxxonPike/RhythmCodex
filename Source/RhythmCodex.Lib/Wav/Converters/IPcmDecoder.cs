using System;

namespace RhythmCodex.Wav.Converters
{
    public interface IPcmDecoder
    {
        float[] Decode8Bit(ReadOnlySpan<byte> bytes);
        float[] Decode16Bit(ReadOnlySpan<byte> bytes);
        float[] Decode24Bit(ReadOnlySpan<byte> bytes);
        float[] Decode32Bit(ReadOnlySpan<byte> bytes);
        float[] DecodeFloat(ReadOnlySpan<byte> bytes);
    }
}