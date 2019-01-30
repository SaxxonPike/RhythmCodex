using System.Collections.Generic;
using System.IO;
using System.Linq;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;
using RhythmCodex.Iso.Model;

namespace RhythmCodex.Iso.Streamers
{
    [Service]
    public class CdSectorStreamReader : ICdSectorStreamReader
    {
        private const int SectorLength = 2352;
        
        public IEnumerable<ICdSector> Read(Stream stream, long length, bool keepOnDisk)
        {
            if (keepOnDisk)
            {
                var reader = new BinaryReader(stream);
                return new CdSectorOnDiskCollection((int)(length / SectorLength), i =>
                {
                    stream.Position = i * (long) SectorLength;
                    return reader.ReadBytes(SectorLength);
                });
            }
            else
            {
                return ReadInternal(stream, length);                
            }
        }

        private static IEnumerable<ICdSector> ReadInternal(Stream stream, long length)
        {
            var offset = 0;
            var reader = new BinaryReader(stream);
            var number = 0;
            
            while (offset < length - SectorLength - 1)
            {
                var sector = reader.ReadBytes(SectorLength);
                offset += SectorLength;
                
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