using System.Collections.Generic;
using RhythmCodex.Archs.Psx.Model;

namespace RhythmCodex.Archs.Psx.Converters;

public interface IBmDataKeysoundBlockDecoder
{
    List<BmDataKeysound> Decode(BmDataKeysoundBlock block);
}