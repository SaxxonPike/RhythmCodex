using System.Collections.Generic;
using RhythmCodex.Archs.Psx.Model;

namespace RhythmCodex.Archs.Psx.Converters;

public interface IPsxMgsDecoder
{
    List<PsxMgsSoundDecodeResult> DecodeSounds(PsxMgsSoundBankBlock soundBank,
        PsxMgsSoundTableBlock soundTable, int sampleRate);
}