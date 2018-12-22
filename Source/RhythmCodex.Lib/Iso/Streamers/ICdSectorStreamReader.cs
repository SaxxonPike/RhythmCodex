using System.Collections.Generic;
using System.IO;
using RhythmCodex.Iso.Model;

namespace RhythmCodex.Iso.Streamers
{
    public interface ICdSectorStreamReader
    {
        IEnumerable<ICdSector> Read(Stream stream, int length, bool keepOnDisk);
    }
}