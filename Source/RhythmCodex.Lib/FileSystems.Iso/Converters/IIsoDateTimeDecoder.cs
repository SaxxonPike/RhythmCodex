using System;

namespace RhythmCodex.FileSystems.Iso.Converters;

public interface IIsoDateTimeDecoder
{
    DateTimeOffset? Decode(ReadOnlySpan<byte> data);
}