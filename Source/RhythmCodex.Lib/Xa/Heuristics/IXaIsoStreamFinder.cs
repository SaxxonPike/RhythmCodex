using System.Collections.Generic;
using RhythmCodex.Iso.Model;
using RhythmCodex.Xa.Models;

namespace RhythmCodex.Xa.Heuristics
{
    public interface IXaIsoStreamFinder
    {
        IList<XaChunk> Find(IEnumerable<IsoSectorInfo> sectors);
    }
}