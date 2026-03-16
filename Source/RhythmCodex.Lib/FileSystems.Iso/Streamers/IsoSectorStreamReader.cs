using System.Collections.Generic;
using System.IO;
using RhythmCodex.FileSystems.Cd.Model;
using RhythmCodex.FileSystems.Cd.Streamers;
using RhythmCodex.FileSystems.Iso.Processors;
using RhythmCodex.IoC;

namespace RhythmCodex.FileSystems.Iso.Streamers;

[Service]
public class IsoSectorStreamReader(IIsoSectorExpander isoSectorExpander) : IIsoSectorStreamReader
{
    private const int InputSectorLength = 2048;

    public IEnumerable<ICdSector> Read(Stream stream, int length, bool keepOnDisk)
    {
        if (keepOnDisk)
        {
            var reader = new BinaryReader(stream);
            return new CdSectorOnDiskCollection(length / InputSectorLength, i =>
            {
                stream.Position = i * (long)InputSectorLength;
                var totalFrame = 150 + i;
                return isoSectorExpander.Expand2048To2352(totalFrame / 4500, totalFrame / 75 % 60, totalFrame % 75,
                    reader.ReadBytes(InputSectorLength));
            });
        }

        return ReadInternal(stream, length);
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
            var data = isoSectorExpander.Expand2048To2352(minute, second, frame, sector);

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