using System.Collections.Generic;
using RhythmCodex.FileSystems.Iso.Model;
using RhythmCodex.Sounds.Xa.Models;

namespace RhythmCodex.Sounds.Xa.Heuristics;

public interface IXaIsoStreamFinder
{
    List<XaChunk> Find(IEnumerable<IsoSectorInfo> sectors);
}