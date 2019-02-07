using System.Collections.Generic;
using RhythmCodex.Cd.Model;
using RhythmCodex.Iso.Model;

namespace RhythmCodex.Iso.Converters
{
    public interface IIsoPathTableDecoder
    {
        IList<Iso9660PathTableEntry> Decode(IEnumerable<ICdSector> sectors);
    }
}