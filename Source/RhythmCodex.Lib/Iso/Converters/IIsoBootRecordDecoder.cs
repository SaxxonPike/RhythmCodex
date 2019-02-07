using System;
using RhythmCodex.Iso.Model;

namespace RhythmCodex.Iso.Converters
{
    public interface IIsoBootRecordDecoder
    {
        Iso9660BootRecord Decode(ReadOnlySpan<byte> data);
    }
}