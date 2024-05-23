using System.Collections.Generic;
using RhythmCodex.Ddr.Models;

namespace RhythmCodex.Ddr.Converters;

public interface IDdrPs2FileDataUnboundTableDecoder
{
    IList<DdrPs2FileDataTableEntry> Decode(DdrPs2FileDataTableChunk? chunk);
}