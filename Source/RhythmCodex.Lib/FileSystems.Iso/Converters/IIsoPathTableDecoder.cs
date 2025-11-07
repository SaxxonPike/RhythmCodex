using System.Collections.Generic;
using RhythmCodex.FileSystems.Cd.Model;
using RhythmCodex.FileSystems.Iso.Model;

namespace RhythmCodex.FileSystems.Iso.Converters;

public interface IIsoPathTableDecoder
{
    List<IsoPathRecord> Decode(IEnumerable<ICdSector> sectors);
}