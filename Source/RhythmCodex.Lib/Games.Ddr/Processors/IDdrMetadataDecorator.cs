using RhythmCodex.Games.Ddr.Models;
using RhythmCodex.Games.Stepmania.Model;

namespace RhythmCodex.Games.Ddr.Processors;

public interface IDdrMetadataDecorator
{
    void Decorate(ChartSet chartSet, DdrDatabaseEntry meta, MetadataDecoratorFileExtensions extensions);
}