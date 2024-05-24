using System.Collections.Generic;
using System.IO;
using RhythmCodex.Cd.Model;

namespace RhythmCodex.Iso.Streamers;

public interface IIsoSectorStreamFactory
{
    Stream Open(IEnumerable<ICdSector> sectors);
    Stream Open(IEnumerable<ICdSector> sectors, long length);
}