using System.Collections.Generic;
using RhythmCodex.Iso.Model;

namespace RhythmCodex.Iso.Converters;

public interface IIsoDescriptorSectorFinder
{
    IList<IsoSectorInfo> Find(IEnumerable<IsoSectorInfo> sectors);
}