using System.Collections.Generic;
using RhythmCodex.Ddr.Models;

namespace RhythmCodex.Ddr.Converters;

public interface IDdrPs2FileDataBoundTableDecoder
{
    List<DdrPs2FileDataTableEntry> Decode(DdrPs2FileDataTableChunk chunk);
}