using System.Collections.Generic;
using RhythmCodex.Archs.Psx.Model;

namespace RhythmCodex.Archs.Psx.Converters;

public interface IPsxMgsSoundTableDecoder
{
    List<PsxMgsSoundScript> Decode(PsxMgsSoundTableBlock block);
}