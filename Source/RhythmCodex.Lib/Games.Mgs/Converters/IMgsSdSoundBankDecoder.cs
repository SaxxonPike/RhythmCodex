using System.Collections.Generic;
using RhythmCodex.Games.Mgs.Models;

namespace RhythmCodex.Games.Mgs.Converters;

public interface IMgsSdSoundBankDecoder
{
    List<MgsSdSoundBankEntryWithData> Decode(MgsSdSoundBankBlock block);
}