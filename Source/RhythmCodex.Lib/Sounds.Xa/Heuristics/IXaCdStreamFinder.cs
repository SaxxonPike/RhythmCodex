using System.Collections.Generic;
using RhythmCodex.FileSystems.Iso.Model;
using RhythmCodex.Sounds.Xa.Models;

namespace RhythmCodex.Sounds.Xa.Heuristics;

public interface IXaCdStreamFinder
{
    IEnumerable<XaChunk> FindMode2(IEnumerable<IsoSectorInfo> sectors);
}