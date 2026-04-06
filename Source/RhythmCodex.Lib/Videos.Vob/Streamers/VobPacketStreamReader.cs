using System.Collections.Generic;
using System.IO;
using System.Linq;
using RhythmCodex.IoC;
using RhythmCodex.Utils.Cursors;
using RhythmCodex.Videos.Vob.Models;

namespace RhythmCodex.Videos.Vob.Streamers;

[Service]
public sealed class VobPacketStreamReader : IVobPacketStreamReader
{
    public IEnumerable<VobPacket> ReadPackets(Stream stream)
    {
        var actualRead = 0x4000;

        while (actualRead == 0x4000)
        {
            var buffer = new byte[0x4000];
            actualRead = stream.ReadAtLeast(buffer, buffer.Length, false);

            var id = buffer.AsS32B();

            if ((id & ~0x000000FF) != 0x00000100)
                break;

            switch (id & 0xFF)
            {
                case 0xB9:
                    yield break;
                case 0xBA:
                    yield return new VobPacket
                    {
                        Data = buffer
                    };
                    break;
            }
        }
    }

    public VobFile ReadFile(Stream stream) =>
        new()
        {
            Packets = ReadPackets(stream).ToList()
        };
}