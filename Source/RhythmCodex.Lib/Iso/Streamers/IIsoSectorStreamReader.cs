using System.Collections.Generic;
using System.IO;
using RhythmCodex.Iso.Model;

namespace RhythmCodex.Iso.Streamers
{
    public interface IIsoSectorStreamReader
    {
        IEnumerable<IsoSector> Read(Stream stream, int length);
    }
}