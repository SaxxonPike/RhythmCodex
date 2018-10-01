using System.Collections.Generic;
using System.IO;
using System.Linq;
using RhythmCodex.Iso.Model;

namespace RhythmCodex.Iso.Streamers
{
    public class IsoSectorStreamReader : IIsoSectorStreamReader
    {
        private const int SectorLength = 2352;
        
        public IEnumerable<IsoSector> Read(Stream stream, int length)
        {
            return ReadInternal(stream, length);
        }

        private static IEnumerable<IsoSector> ReadInternal(Stream stream, int length)
        {
            var offset = 0;
            var reader = new BinaryReader(stream);
            var number = 0;
            
            while (offset < length - SectorLength - 1)
            {
                var sector = reader.ReadBytes(SectorLength);
                offset += SectorLength;
                
                yield return new IsoSector
                {
                    Data = sector,
                    Number = number
                };

                number++;
            }            
        }
    }
}