using RhythmCodex.Ddr.Models;

namespace RhythmCodex.Ddr.Converters;

public interface IDdrPs2DatabaseDecoder
{
    DdrDatabaseEntry? Decode(DdrPs2MetadataTableEntry item);
}