using System;
using RhythmCodex.Charts.Ssq.Model;

namespace RhythmCodex.Charts.Step1.Converters;

public interface IStep1TimingChunkDecoder
{
    TimingChunk Convert(ReadOnlySpan<byte> data);
}