using System.Collections.Generic;
using RhythmCodex.Sounds.Models;

namespace RhythmCodex.Games.Mgs.Models;

public struct MgsSdSoundDecodeResult
{
    public int Index { get; init; }
    public Sound Sound { get; init; }
    public Dictionary<int, List<MgsSdSoundTablePacket>> Packets { get; init; }
}