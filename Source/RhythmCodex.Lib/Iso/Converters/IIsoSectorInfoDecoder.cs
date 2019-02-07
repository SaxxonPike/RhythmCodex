using RhythmCodex.Cd.Model;
using RhythmCodex.Iso.Model;

namespace RhythmCodex.Iso.Converters
{
    public interface IIsoSectorInfoDecoder
    {
        Iso9660SectorInfo Decode(ICdSector sector);
    }
}