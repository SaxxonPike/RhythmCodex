using System.Collections.Generic;
using RhythmCodex.Ddr.Models;

namespace RhythmCodex.Ddr.Converters;

public interface IDdrPs2FileDataUnboundTableDecoder
{
    List<DdrPs2FileDataTableEntry> Decode(DdrPs2FileDataTableChunk? chunk);
}