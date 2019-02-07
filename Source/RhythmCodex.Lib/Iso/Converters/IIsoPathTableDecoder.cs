using System.Collections.Generic;
using RhythmCodex.Cd.Model;
using RhythmCodex.Iso.Model;

namespace RhythmCodex.Iso.Converters
{
    public interface IIsoPathTableDecoder
    {
        IList<IsoPathRecord> Decode(IEnumerable<ICdSector> sectors);
    }
}