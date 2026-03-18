using System;
using RhythmCodex.FileSystems.Cd.Model;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;

namespace RhythmCodex.FileSystems.Iso.Processors;

[Service]
public sealed class IsoSectorConverter : IIsoSectorConverter
{
    public byte[] ConvertCookedToRawSector(int minute, int second, int frame, int mode, ReadOnlySpan<byte> sector)
    {
        if (sector.Length > CdSector.CookedSectorSize)
            return sector.ToArray();

        var data = new byte[CdSector.RawSectorSize];
        sector[..CdSector.CookedSectorSize].CopyTo(data.AsSpan(0x0010));
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