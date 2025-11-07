using System.Collections.Generic;
using RhythmCodex.FileSystems.Cd.Model;
using RhythmCodex.FileSystems.Iso.Model;

namespace RhythmCodex.FileSystems.Iso.Converters;

public interface IIsoDirectoryTableDecoder
{
    List<IsoDirectoryRecord> Decode(IEnumerable<ICdSector> cdSectors);
}