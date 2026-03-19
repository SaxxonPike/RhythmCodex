using System;

namespace RhythmCodex.Archs.Psx.Model;

public class PsxMgsSoundTablePacket
{
    public PsxMgsSoundTablePacketType Command { get; set; } // mdata1
    public byte Data2 { get; set; } // mdata2
    public byte Data3 { get; set; } // mdata3
    public byte Data4 { get; set; } // mdata4
}