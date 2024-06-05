using System;
using RhythmCodex.Xbox.Model;

namespace RhythmCodex.Xbox.Converters;

public interface IXboxIsoInfoDecoder
{
    XboxIsoInfo Decode(ReadOnlySpan<byte> sector);
}