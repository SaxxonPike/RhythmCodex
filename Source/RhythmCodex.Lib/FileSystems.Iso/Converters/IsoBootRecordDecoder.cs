using System;
using RhythmCodex.FileSystems.Iso.Model;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;

namespace RhythmCodex.FileSystems.Iso.Converters;

[Service]
public class IsoBootRecordDecoder : IIsoBootRecordDecoder
{
    public IsoBootRecord Decode(ReadOnlySpan<byte> data)
    {
        return new IsoBootRecord
        {
            BootSystemIdentifier = Encodings.Cp437.GetString(data.Slice(7, 32)),
            BootIdentifier = Encodings.Cp437.GetString(data.Slice(39, 32)),
            BootSystemData = data.Slice(71, 1977).ToArray()
        };
    }
}