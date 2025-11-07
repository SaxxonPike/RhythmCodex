using RhythmCodex.Ddr.Models;
using RhythmCodex.Ddr.Ps2.Models;

namespace RhythmCodex.Ddr.Ps2.Converters;

public interface IDdrPs2DatabaseDecoder
{
    DdrDatabaseEntry? Decode(DdrPs2MetadataTableEntry item);
}