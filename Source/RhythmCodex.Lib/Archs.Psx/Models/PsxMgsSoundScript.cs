using System.Collections.Generic;

namespace RhythmCodex.Archs.Psx.Model;

public record PsxMgsSoundScript
{
    public int Index { get; set; }
    // public int Flags { get; set; }
    public byte Priority { get; set; }
    public byte ChannelCount { get; set; }
    public byte Kind { get; set; }
    public byte UniqueGroup { get; set; }
    public Dictionary<int, List<PsxMgsSoundTablePacket>> Channels { get; set; } = [];
}