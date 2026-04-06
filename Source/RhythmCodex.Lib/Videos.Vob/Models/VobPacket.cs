using System;

namespace RhythmCodex.Videos.Vob.Models;

public class VobPacket
{
    public Memory<byte> Data { get; init; }
}