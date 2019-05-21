using System.Collections.Generic;
using System.IO;
using RhythmCodex.Cd.Model;
using RhythmCodex.IoC;

namespace RhythmCodex.Cd.Streamers
{
    [Service]
    public class CdSectorStreamReader : ICdSectorStreamReader
    {
        public IEnumerable<ICdSector> Read(Stream stream, long length, bool keepOnDisk, int sectorLength = 2352)
        {
            if (keepOnDisk)
            {
                var reader = new BinaryReader(stream);
                return new CdSectorOnDiskCollection((int)(length / sectorLength), i =>
                {
                    stream.Position = i * (long) sectorLength;
                    return reader.ReadBytes(sectorLength);
                });
            }

            return ReadInternal(stream, length, sectorLength);
        }

        private static IEnumerable<ICdSector> ReadInternal(Stream stream, long length, int sectorLength)
        {
            var offset = 0;
            var reader = new BinaryReader(stream);
            var number = 0;
            
            while (offset < length - sectorLength - 1)
            {
                var sector = reader.ReadBytes(sectorLength);
                offset += sectorLength;
                
                yield return new CdSector
                {
                    Data = sector,
                    Number = number
                };

                number++;
            }            
        }
    }
}