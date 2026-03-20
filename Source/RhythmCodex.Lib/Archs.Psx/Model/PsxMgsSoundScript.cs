using System.Collections.Generic;

namespace RhythmCodex.Archs.Psx.Model;

public record PsxMgsSoundScript
{
    public int Index { get; set; }
    public int Flags { get; set; }
    public Dictionary<int, List<PsxMgsSoundTablePacket>> Channels { get; set; } = [];
}