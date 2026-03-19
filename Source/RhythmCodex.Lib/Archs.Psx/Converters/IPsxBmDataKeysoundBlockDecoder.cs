using System.Collections.Generic;
using RhythmCodex.Archs.Psx.Model;

namespace RhythmCodex.Archs.Psx.Converters;

public interface IPsxBmDataKeysoundBlockDecoder
{
    List<PsxBmDataKeysound> Decode(PsxBmDataKeysoundBlock block);
}