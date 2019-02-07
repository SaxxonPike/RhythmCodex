using System;

namespace RhythmCodex.Iso.Converters
{
    public interface IIsoDateTimeDecoder
    {
        DateTimeOffset? Decode(ReadOnlySpan<byte> data);
    }
}