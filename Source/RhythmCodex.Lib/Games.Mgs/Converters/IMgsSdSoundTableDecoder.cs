using System.Collections.Generic;
using RhythmCodex.Games.Mgs.Models;

namespace RhythmCodex.Games.Mgs.Converters;

public interface IMgsSdSoundTableDecoder
{
    List<MgsSdSoundScript> Decode(MgsSdSoundTableBlock block);
}