using RhythmCodex.Games.Ddr.Models;
using RhythmCodex.Games.Ddr.Ps2.Models;

namespace RhythmCodex.Games.Ddr.Ps2.Converters;

public interface IDdrPs2DatabaseDecoder
{
    DdrDatabaseEntry? Decode(DdrPs2MetadataTableEntry item);
}