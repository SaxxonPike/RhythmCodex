using RhythmCodex.Charts.Models;
using RhythmCodex.Games.Ddr.Models;

namespace RhythmCodex.Games.Ddr.Processors;

public interface IDdrMetadataDecorator
{
    void Decorate(ChartSet chartSet, DdrDatabaseEntry meta, MetadataDecoratorFileExtensions extensions);
}