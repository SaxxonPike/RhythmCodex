using RhythmCodex.Ddr.Models;
using RhythmCodex.Stepmania.Model;

namespace RhythmCodex.Ddr.Processors
{
    public interface IDdrMetadataDecorator
    {
        void Decorate(ChartSet chartSet, DdrDatabaseEntry meta, MetadataDecoratorFileExtensions extensions);
    }
}