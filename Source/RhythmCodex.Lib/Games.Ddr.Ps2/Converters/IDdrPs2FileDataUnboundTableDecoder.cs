using System.Collections.Generic;
using RhythmCodex.Games.Ddr.Ps2.Models;

namespace RhythmCodex.Games.Ddr.Ps2.Converters;

public interface IDdrPs2FileDataUnboundTableDecoder
{
    List<DdrPs2FileDataTableEntry> Decode(DdrPs2FileDataTableChunk? chunk);
}