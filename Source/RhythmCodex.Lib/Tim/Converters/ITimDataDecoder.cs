using System;

namespace RhythmCodex.Tim.Converters;

public interface ITimDataDecoder
{
    int[] Decode4Bit(ReadOnlySpan<byte> data, int stride, int height);
    int[] Decode8Bit(ReadOnlySpan<byte> data, int stride, int height);
    int[] Decode16Bit(ReadOnlySpan<byte> data, int stride, int height);
    int[] Decode24Bit(ReadOnlySpan<byte> data, int stride, int height);
}