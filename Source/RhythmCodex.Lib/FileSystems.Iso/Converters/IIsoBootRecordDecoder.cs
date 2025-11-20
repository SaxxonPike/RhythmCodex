using System;
using RhythmCodex.FileSystems.Iso.Model;

namespace RhythmCodex.FileSystems.Iso.Converters;

public interface IIsoBootRecordDecoder
{
    IsoBootRecord Decode(ReadOnlySpan<byte> data);
}