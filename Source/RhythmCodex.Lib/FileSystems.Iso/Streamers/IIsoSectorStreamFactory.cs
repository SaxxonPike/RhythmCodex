using System.Collections.Generic;
using System.IO;
using RhythmCodex.FileSystems.Cd.Model;

namespace RhythmCodex.FileSystems.Iso.Streamers;

public interface IIsoSectorStreamFactory
{
    Stream Open(IEnumerable<ICdSector> sectors);
    Stream Open(IEnumerable<ICdSector> sectors, long length);
}