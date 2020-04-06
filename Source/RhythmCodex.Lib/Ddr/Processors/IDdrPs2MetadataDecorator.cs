using RhythmCodex.Ddr.Models;
using RhythmCodex.Stepmania.Model;

namespace RhythmCodex.Ddr.Processors
{
    public interface IDdrPs2MetadataDecorator
    {
        void Decorate(ChartSet chartSet, DdrDatabaseEntry meta);
    }
}