using System.Collections.Generic;
using RhythmCodex.Games.Mgs.Models;
using RhythmCodex.IoC;
using RhythmCodex.Utils.Cursors;

namespace RhythmCodex.Games.Mgs.Converters;

/// <inheritdoc />
[Service]
public sealed class MgsSdSoundTableDecoder
    : IMgsSdSoundTableDecoder
{
    public List<MgsSdSoundScript> Decode(MgsSdSoundTableBlock block)
    {
        //
        // Parse the scripts according to the table.
        //

        var tableSpan = block.Table.Span;
        var scriptSpan = block.Scripts.Span;
        var result = new List<MgsSdSoundScript>();

        for (var i = 0; i < tableSpan.Length; i += 16)
        {
            var record = tableSpan.Slice(i, 16);
            var offsets = new[]
            {
                record[4..].AsS32L(),
                record[8..].AsS32L()
            };

            var packetSets = new Dictionary<int, List<MgsSdSoundTablePacket>>();

            for (var j = 0; j < offsets.Length; j++)
            {
                var offset = offsets[j];
                
                if (offset < 0)
                    continue;

                var packets = new List<MgsSdSoundTablePacket>();
                var packetSpan = scriptSpan[offset..];

                while (packetSpan.Length >= 4)
                {
                    var packet = new MgsSdSoundTablePacket
                    {
                        Data1 = packetSpan[3],
                        Data2 = packetSpan[2],
                        Data3 = packetSpan[1],
                        Data4 = packetSpan[0]
                    };

                    packetSpan = packetSpan[4..];

                    packets.Add(packet);
                    if (packet.Command == MgsSdSoundTablePacketType.End)
                        break;
                }

                packetSets[j] = packets;
            }

            var script = new MgsSdSoundScript
            {
                Index = (i >> 4) + 128,
                Channels = packetSets,
                Priority = record[0], // SETBL.pri
                ChannelCount = record[1], // SETBL.tracks
                Kind = record[2], // SETBL.kind
                UniqueGroup = record[3] // SETBL.character
            };

            result.Add(script);
        }

        return result;
    }
}