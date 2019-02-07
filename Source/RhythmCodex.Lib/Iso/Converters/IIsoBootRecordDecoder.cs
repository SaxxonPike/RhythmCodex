using System;
using RhythmCodex.Iso.Model;

namespace RhythmCodex.Iso.Converters
{
    public interface IIsoBootRecordDecoder
    {
        IsoBootRecord Decode(ReadOnlySpan<byte> data);
    }
}