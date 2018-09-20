using RhythmCodex.Iso.Model;

namespace RhythmCodex.Iso.Converters
{
    public interface IIsoSectorInfoDecoder
    {
        IsoSectorInfo Decode(IsoSector sector);
    }
}