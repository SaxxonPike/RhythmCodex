using RhythmCodex.Archs.Psx.Model;
using RhythmCodex.Sounds.Models;

namespace RhythmCodex.Archs.Psx.Converters;

public interface IBmDataKeysoundDecoder
{
    Sound Decode(BmDataKeysound keysound);
}