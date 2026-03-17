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
    public IEnumerable<ICdSector> Read(Stream stream, int length, bool keepOnDisk, bool raw, int? mode)
    {
        var inputSectorLength = raw ? 2352 : 2048;

        if (keepOnDisk)
        {
            var reader = new BinaryReader(stream);
            return new DeferredCdSectorCollection(length / inputSectorLength, i =>
            {
                stream.Position = i * (long)inputSectorLength;
                var totalFrame = 150 + i;
                
                return isoSectorExpander.Expand2048To2352(
                    totalFrame / 4500, totalFrame / 75 % 60, totalFrame % 75,
                    mode ?? 1,
                    reader.ReadBytes(inputSectorLength)
                );
            });
        }

        return ReadInternal(stream, length, inputSectorLength, mode ?? 1);
    }

    private IEnumerable<ICdSector> ReadInternal(Stream stream, int length, int inputSectorLength, int mode)
    {
        var offset = 0;
        var reader = new BinaryReader(stream);
        var number = 0;
        byte minute = 0;
        byte second = 2;
        byte frame = 0;

        while (offset < length - inputSectorLength - 1)
        {
            var sector = reader.ReadBytes(inputSectorLength);
            offset += inputSectorLength;

            var data = isoSectorExpander.Expand2048To2352(
                minute, second, frame, mode, sector
            );

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