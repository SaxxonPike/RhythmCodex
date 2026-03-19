using System.Collections.Generic;

namespace RhythmCodex.Archs.Psx.Model;

public class PsxMgsSoundScript
{
    public int Index { get; set; }
    public List<PsxMgsSoundTablePacket> Packets { get; set; } = [];
}