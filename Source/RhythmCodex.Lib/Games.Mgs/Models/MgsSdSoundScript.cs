using System.Collections.Generic;

namespace RhythmCodex.Games.Mgs.Models;

public record MgsSdSoundScript
{
    public int Index { get; init; }
    public byte Priority { get; init; }
    public byte ChannelCount { get; init; }
    public byte Kind { get; init; }
    public byte UniqueGroup { get; init; }
    public Dictionary<int, List<MgsSdSoundTablePacket>> Channels { get; init; } = [];
}