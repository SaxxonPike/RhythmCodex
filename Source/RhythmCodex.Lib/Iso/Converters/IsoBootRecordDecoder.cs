using System;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;
using RhythmCodex.Iso.Model;

namespace RhythmCodex.Iso.Converters
{
    [Service]
    public class IsoBootRecordDecoder : IIsoBootRecordDecoder
    {
        public IsoBootRecord Decode(ReadOnlySpan<byte> data)
        {
            return new IsoBootRecord
            {
                BootSystemIdentifier = Encodings.CP437.GetString(data.Slice(7, 32)),
                BootIdentifier = Encodings.CP437.GetString(data.Slice(39, 32)),
                BootSystemData = data.Slice(71, 1977).ToArray()
            };
        }
    }
}