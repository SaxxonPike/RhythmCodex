using System.Collections.Generic;
using System.IO;
using RhythmCodex.Cd.Model;

namespace RhythmCodex.Iso.Streamers
{
    public interface IIsoSectorStreamReader
    {
        IEnumerable<ICdSector> Read(Stream stream, int length, bool keepOnDisk);
    }
}