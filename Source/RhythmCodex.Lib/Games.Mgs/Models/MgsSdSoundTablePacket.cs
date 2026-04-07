namespace RhythmCodex.Games.Mgs.Models;

public record MgsSdSoundTablePacket
{
    public byte Data1 { get; init; } // mdata1
    public byte Data2 { get; init; } // mdata2
    public byte Data3 { get; init; } // mdata3
    public byte Data4 { get; init; } // mdata4

    public MgsSdSoundTablePacketType Command => 
        (MgsSdSoundTablePacketType)Data1;
}