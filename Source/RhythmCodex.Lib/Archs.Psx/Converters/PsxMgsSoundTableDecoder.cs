using System;
using System.Collections.Generic;
using RhythmCodex.Archs.Psx.Model;
using RhythmCodex.IoC;

namespace RhythmCodex.Archs.Psx.Converters;

/// <inheritdoc />
[Service]
public class PsxMgsSoundTableDecoder
    : IPsxMgsSoundTableDecoder
{
    public List<PsxMgsSoundScript> Decode(PsxMgsSoundTableBlock block)
    {
        //
        // The signature used to determine the start of the table is:
        // xxxxxxxx 00000000 FFFFFFFF FFFFFFFF
        //

        Span<byte> startCheck = stackalloc byte[12];
        startCheck[4..].Fill(0xFF);

        var span = block.Data.Span;

        //
        // Find the beginning of the table.
        //

        while (span.Length >= 16)
        {
            if (span[4..16].SequenceEqual(startCheck))
                break;
        }

        //
        // Find the beginning of the script block.
        //

        var tableSize = 0;
        while (tableSize < span.Length - 15)
        {
            if (!span[8..16].SequenceEqual(startCheck[4..]))
                break;
            tableSize += 16;
        }

        //
        // Parse the scripts according to the table.
        //

        var tableSpan = span[..tableSize];
        var scriptSpan = span[tableSize..];
        var result = new List<PsxMgsSoundScript>();

        for (var i = 0; i < tableSpan.Length; i += 16)
        {
            var record = tableSpan.Slice(i, 16);
            var flags = ReadInt32LittleEndian(record);
            var offset = ReadInt32LittleEndian(record[4..]);

            if (offset < 0)
                continue;

            var packets = new List<PsxMgsSoundTablePacket>();
            var packetSpan = scriptSpan[offset..];

            while (packetSpan.Length >= 4)
            {
                var packet = new PsxMgsSoundTablePacket
                {
                    Command = (PsxMgsSoundTablePacketType)packetSpan[3],
                    Data2 = packetSpan[2],
                    Data3 = packetSpan[1],
                    Data4 = packetSpan[0]
                };

                packetSpan = packetSpan[4..];

                packets.Add(packet);
                if (packet.Command == PsxMgsSoundTablePacketType.EndBlock)
                    break;
            }

            var script = new PsxMgsSoundScript
            {
                Index = i >> 4,
                Packets = packets,
                Flags = flags
            };

            result.Add(script);
        }

        return result;
    }
}