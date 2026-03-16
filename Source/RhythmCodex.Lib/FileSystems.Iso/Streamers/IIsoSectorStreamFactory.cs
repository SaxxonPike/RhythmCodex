using System.Collections.Generic;
using System.IO;
using RhythmCodex.FileSystems.Cd.Model;

namespace RhythmCodex.FileSystems.Iso.Streamers;

public interface IIsoSectorStreamFactory
{
    Stream Open(IEnumerable<ICdSector> sectors);
    Stream Open(IEnumerable<ICdSector> sectors, long length);
    Stream OpenRaw(IEnumerable<ICdSector> sectors);
    Stream OpenRaw(IEnumerable<ICdSector> sectors, long length);
}