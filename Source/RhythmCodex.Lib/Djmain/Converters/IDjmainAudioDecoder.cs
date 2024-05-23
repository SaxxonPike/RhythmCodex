using System;
using System.Collections.Generic;

namespace RhythmCodex.Djmain.Converters;

public interface IDjmainAudioDecoder
{
    float[] DecodeDpcm(ReadOnlySpan<byte> data);
    float[] DecodePcm8(ReadOnlySpan<byte> data);
    float[] DecodePcm16(ReadOnlySpan<byte> data);
}