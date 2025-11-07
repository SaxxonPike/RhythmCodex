using System.Collections.Generic;
using RhythmCodex.Ddr.Ps2.Models;

namespace RhythmCodex.Ddr.Ps2.Converters;

public interface IDdrPs2FileDataUnboundTableDecoder
{
    List<DdrPs2FileDataTableEntry> Decode(DdrPs2FileDataTableChunk? chunk);
}