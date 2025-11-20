using RhythmCodex.FileSystems.Cd.Model;
using RhythmCodex.FileSystems.Iso.Model;

namespace RhythmCodex.FileSystems.Iso.Converters;

public interface IIsoSectorInfoDecoder
{
    IsoSectorInfo Decode(ICdSector sector);
}