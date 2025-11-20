using System;
using RhythmCodex.Archs.Xbox.Model;

namespace RhythmCodex.Archs.Xbox.Converters;

public interface IXboxIsoInfoDecoder
{
    XboxIsoInfo Decode(ReadOnlySpan<byte> sector);
}