using System;
using RhythmCodex.Twinkle.Model;

namespace RhythmCodex.Twinkle.Converters;

public interface ITwinkleBeatmaniaSoundDefinitionDecoder
{
    TwinkleBeatmaniaSoundDefinition Decode(ReadOnlySpan<byte> data);
}