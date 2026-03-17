using System;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;

namespace RhythmCodex.FileSystems.Iso.Processors;

[Service]
public sealed class IsoSectorExpander : IIsoSectorExpander
{
    public byte[] Expand2048To2352(int minute, int second, int frame, int mode, ReadOnlySpan<byte> sector)
    {
        if (sector.Length > 2048)
            return sector.ToArray();

        var data = new byte[2352];
        sector[..2048].CopyTo(data.AsSpan(0x0010));
        new byte[]
        {
            0x00, 0xFF, 0xFF, 0xFF,
            0xFF, 0xFF, 0xFF, 0xFF,
            0xFF, 0xFF, 0xFF, 0x00,
            Bcd.Encode(minute), Bcd.Encode(second), Bcd.Encode(frame), (byte)mode
        }.AsSpan().CopyTo(data);
        return data;
    }
}