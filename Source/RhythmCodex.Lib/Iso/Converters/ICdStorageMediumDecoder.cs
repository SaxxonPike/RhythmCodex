using System.Collections.Generic;
using RhythmCodex.Iso.Model;

namespace RhythmCodex.Iso.Converters
{
    public interface ICdStorageMediumDecoder
    {
        Iso9660StorageMedium Decode(IEnumerable<Iso9660SectorInfo> sectors);
    }
}