using RhythmCodex.Ddr.Models;

namespace RhythmCodex.Ddr.Processors
{
    public interface IDdrMetadataDatabase
    {
        DdrMetadataDatabaseEntry GetByCode(string code);
        DdrMetadataDatabaseEntry GetById(int id);
    }
}