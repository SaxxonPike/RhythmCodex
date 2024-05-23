using System.Collections.Generic;
using RhythmCodex.Cd.Model;
using RhythmCodex.Iso.Model;

namespace RhythmCodex.Iso.Converters;

public interface IIsoDirectoryTableDecoder
{
    IList<IsoDirectoryRecord> Decode(IEnumerable<ICdSector> cdSectors);
}