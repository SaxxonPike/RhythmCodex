using System.Collections.Generic;
using System.IO;
using RhythmCodex.Videos.Vob.Models;

namespace RhythmCodex.Videos.Vob.Streamers;

public interface IVobPacketStreamReader
{
    IEnumerable<VobPacket> ReadPackets(Stream stream);
    VobFile ReadFile(Stream stream);
}