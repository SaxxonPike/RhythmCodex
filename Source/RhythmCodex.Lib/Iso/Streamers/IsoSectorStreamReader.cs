using System.Collections.Generic;
using System.IO;
using System.Linq;
using RhythmCodex.Iso.Model;

namespace RhythmCodex.Iso.Streamers
{
    public class IsoSectorStreamReader : IIsoSectorStreamReader
    {
        public IEnumerable<IsoSector> Read(Stream stream, int length)
        {
            return ReadInternal(stream, length);
        }

        private IEnumerable<IsoSector> ReadInternal(Stream stream, int length)
        {
            var offset = 0;
            var reader = new BinaryReader(stream);
            var number = 0;
            
            while (offset < length - 2351)
            {
                var sector = reader.ReadBytes(2352);
                offset += 2352;
                
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