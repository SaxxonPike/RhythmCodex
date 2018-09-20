using System.Collections.Generic;
using RhythmCodex.Iso.Model;

namespace RhythmCodex.Iso.Converters
{
    public interface IIsoStorageMediumDecoder
    {
        IList<IsoStorageMedium> Decode(IEnumerable<IsoSectorInfo> sectors);
    }
}