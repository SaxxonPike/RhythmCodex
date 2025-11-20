using System;
using RhythmCodex.Archs.Xbox.Model;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;

namespace RhythmCodex.Archs.Xbox.Converters;

[Service]
public class XboxIsoInfoDecoder : IXboxIsoInfoDecoder
{
    public XboxIsoInfo Decode(ReadOnlySpan<byte> sector)
    {
        return new XboxIsoInfo
        {
            DirectorySectorNumber = Bitter.ToInt32(sector, 0x14),
            DirectorySize = Bitter.ToInt32(sector, 0x18)
        };
    }
}