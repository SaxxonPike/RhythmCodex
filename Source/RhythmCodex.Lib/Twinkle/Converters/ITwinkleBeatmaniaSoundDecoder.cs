using System;
using RhythmCodex.Sounds.Models;
using RhythmCodex.Twinkle.Model;

namespace RhythmCodex.Twinkle.Converters;

public interface ITwinkleBeatmaniaSoundDecoder
{
    ISound Decode(TwinkleBeatmaniaSoundDefinition definition, ReadOnlySpan<byte> data);
}