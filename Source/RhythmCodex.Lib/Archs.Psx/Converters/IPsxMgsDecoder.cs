using System.Collections.Generic;
using RhythmCodex.Archs.Psx.Model;
using RhythmCodex.Sounds.Models;

namespace RhythmCodex.Archs.Psx.Converters;

public interface IPsxMgsDecoder
{
    List<Sound> DecodeSounds(PsxMgsSoundBankBlock soundBank,
        PsxMgsSoundTableBlock soundTable, int sampleRate);
}