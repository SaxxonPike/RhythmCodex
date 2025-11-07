using System;
using RhythmCodex.Archs.Twinkle.Model;

namespace RhythmCodex.Archs.Twinkle.Converters;

public interface ITwinkleBeatmaniaSoundDefinitionDecoder
{
    TwinkleBeatmaniaSoundDefinition? Decode(ReadOnlySpan<byte> data);
}