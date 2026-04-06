using System.Collections.Generic;

namespace RhythmCodex.Videos.Vob.Models;

public class VobFile
{
    public List<VobPacket> Packets { get; set; } = [];
}