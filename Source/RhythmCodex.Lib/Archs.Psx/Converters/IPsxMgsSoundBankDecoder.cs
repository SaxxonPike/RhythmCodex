using System.Collections.Generic;
using RhythmCodex.Archs.Psx.Model;

namespace RhythmCodex.Archs.Psx.Converters;

public interface IPsxMgsSoundBankDecoder
{
    List<PsxBeatmaniaKeysound> Decode(PsxMgsSoundBankBlock block);
}