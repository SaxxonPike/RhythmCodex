using RhythmCodex.Iso.Model;

namespace RhythmCodex.Iso.Converters
{
    public interface ICdSectorInfoDecoder
    {
        Iso9660SectorInfo Decode(ICdSector sector);
    }
}