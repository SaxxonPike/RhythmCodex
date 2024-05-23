using System;
using System.Runtime.InteropServices.Marshalling;
using RhythmCodex.Infrastructure;

namespace RhythmCodex.Wav.Models;

[Model]
public class ImaAdpcmFormat(ReadOnlySpan<byte> data)
{
    public int SamplesPerBlock { get; set; } = Bitter.ToInt16(data, 2);
}