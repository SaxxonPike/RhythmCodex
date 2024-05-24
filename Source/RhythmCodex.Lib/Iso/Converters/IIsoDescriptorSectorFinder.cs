using System.Collections.Generic;
using RhythmCodex.Iso.Model;

namespace RhythmCodex.Iso.Converters;

public interface IIsoDescriptorSectorFinder
{
    List<IsoSectorInfo> Find(IEnumerable<IsoSectorInfo> sectors);
}