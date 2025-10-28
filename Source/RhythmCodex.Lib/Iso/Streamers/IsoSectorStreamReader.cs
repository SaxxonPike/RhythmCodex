using System;
using System.Collections.Generic;
using System.IO;
using RhythmCodex.Cd.Model;
using RhythmCodex.Cd.Streamers;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;

namespace RhythmCodex.Iso.Streamers;

[Service]
public class IsoSectorStreamReader : IIsoSectorStreamReader
{
    private const int InputSectorLength = 2048;
    private const int OutputSectorLength = 2352;

    public IEnumerable<ICdSector> Read(Stream stream, int length, bool keepOnDisk)
    {
        if (keepOnDisk)
        {
            var reader = new BinaryReader(stream);
            return new CdSectorOnDiskCollection(length / InputSectorLength, i =>
            {
                stream.Position = i * (long) InputSectorLength;
                var totalFrame = 150 + i;
                return ExpandSector(totalFrame / 4500, totalFrame / 75 % 60, totalFrame % 75,
                    reader.ReadBytes(InputSectorLength));
            });
        }

        return ReadInternal(stream, length);
    }

    private byte[] ExpandSector(int minute, int second, int frame, ReadOnlySpan<byte> sector)
    {
        var data = new byte[OutputSectorLength];
        sector[..InputSectorLength].CopyTo(data.AsSpan(0x0010));
        new byte[]
        {
            0x00, 0xFF, 0xFF, 0xFF,
            0xFF, 0xFF, 0xFF, 0xFF,
            0xFF, 0xFF, 0xFF, 0x00,
            Bcd.Encode(minute), Bcd.Encode(second), Bcd.Encode(frame), 0x01
        }.AsSpan().CopyTo(data);
        return data;
    }

    private IEnumerable<ICdSector> ReadInternal(Stream stream, int length)
    {
        var offset = 0;
        var reader = new BinaryReader(stream);
        var number = 0;
        byte minute = 0;
        byte second = 2;
        byte frame = 0;

        while (offset < length - InputSectorLength - 1)
        {
            var sector = reader.ReadBytes(InputSectorLength);
            offset += InputSectorLength;
            var data = ExpandSector(minute, second, frame, sector);

            if (++frame == 75)
            {
                frame = 0;
                second++;
                if (second == 60)
                {
                    second = 0;
                    minute++;
                }
            }

            yield return new CdSector
            {
                Data = data,
                Number = number
            };

            number++;
        }
    }
}