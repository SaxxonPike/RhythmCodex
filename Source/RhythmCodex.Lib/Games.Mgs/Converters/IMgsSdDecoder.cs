using System.Collections.Generic;
using RhythmCodex.Games.Mgs.Models;

namespace RhythmCodex.Games.Mgs.Converters;

public interface IMgsSdDecoder
{
    List<MgsSdSoundDecodeResult> DecodeSounds(MgsSdSoundBankBlock soundBank,
        MgsSdSoundTableBlock soundTable, int sampleRate);
}