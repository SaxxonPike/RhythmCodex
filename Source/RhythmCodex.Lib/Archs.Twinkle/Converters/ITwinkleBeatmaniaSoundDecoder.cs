using System;
using RhythmCodex.Archs.Twinkle.Model;
using RhythmCodex.Sounds.Models;

namespace RhythmCodex.Archs.Twinkle.Converters;

public interface ITwinkleBeatmaniaSoundDecoder
{
    Sound? Decode(TwinkleBeatmaniaSoundDefinition definition, ReadOnlySpan<byte> data);
}