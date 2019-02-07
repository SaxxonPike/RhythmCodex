using RhythmCodex.Cd.Model;
using RhythmCodex.Iso.Model;

namespace RhythmCodex.Iso.Converters
{
    public interface IIsoSectorInfoDecoder
    {
        IsoSectorInfo Decode(ICdSector sector);
    }
}