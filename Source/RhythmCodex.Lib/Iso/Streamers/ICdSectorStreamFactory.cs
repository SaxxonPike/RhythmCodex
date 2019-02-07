using System.Collections.Generic;
using System.IO;
using RhythmCodex.Iso.Model;

namespace RhythmCodex.Iso.Streamers
{
    public interface ICdSectorStreamFactory
    {
        Stream Open(IEnumerable<ICdSector> sectors);
    }
}