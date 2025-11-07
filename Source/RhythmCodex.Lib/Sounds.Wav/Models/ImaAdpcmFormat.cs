using System;
using RhythmCodex.Infrastructure;

namespace RhythmCodex.Sounds.Wav.Models;

[Model]
public class ImaAdpcmFormat(ReadOnlySpan<byte> data)
{
    public int SamplesPerBlock { get; set; } = Bitter.ToInt16(data, 2);
}