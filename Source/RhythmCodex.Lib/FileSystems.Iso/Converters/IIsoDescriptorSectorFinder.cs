using System.Collections.Generic;
using RhythmCodex.FileSystems.Iso.Model;

namespace RhythmCodex.FileSystems.Iso.Converters;

public interface IIsoDescriptorSectorFinder
{
    List<IsoSectorInfo> Find(IEnumerable<IsoSectorInfo> sectors);
}