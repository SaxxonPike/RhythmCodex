using RhythmCodex.Games.Ddr.Models;

namespace RhythmCodex.Games.Ddr.Processors;

public interface IDdrMetadataDatabase
{
    DdrMetadataDatabaseEntry? GetByCode(string code);
    DdrMetadataDatabaseEntry? GetById(int id);
}