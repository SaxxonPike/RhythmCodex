using System;

namespace RhythmCodex.Archs.Psx.Model;

public record PsxMgsSoundTablePacket
{
    public byte Data1 { get; set; } // mdata1
    public byte Data2 { get; set; } // mdata2
    public byte Data3 { get; set; } // mdata3
    public byte Data4 { get; set; } // mdata4

    public PsxMgsSoundTablePacketType Command
    {
        get => (PsxMgsSoundTablePacketType)Data1;
        set => Data1 = unchecked((byte)value);
    }
}