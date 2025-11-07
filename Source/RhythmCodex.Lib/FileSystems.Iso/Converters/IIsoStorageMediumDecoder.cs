using System.Collections.Generic;
using RhythmCodex.FileSystems.Iso.Model;

namespace RhythmCodex.FileSystems.Iso.Converters;

public interface IIsoStorageMediumDecoder
{
    IsoStorageMedium Decode(IEnumerable<IsoSectorInfo> sectors);
}