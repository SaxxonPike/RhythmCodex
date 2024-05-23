using System;
using RhythmCodex.Sounds.Models;
using RhythmCodex.Twinkle.Model;

namespace RhythmCodex.Twinkle.Converters;

public interface ITwinkleBeatmaniaSoundDecoder
{
    Sound? Decode(TwinkleBeatmaniaSoundDefinition definition, ReadOnlySpan<byte> data);
}