using System;
using System.Collections.Generic;
using System.IO;
using RhythmCodex.Games.Beatmania.Ps2.Models;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;
using RhythmCodex.Utils.Cursors;

namespace RhythmCodex.Games.Beatmania.Ps2.Streamers;

[Service]
public sealed class BeatmaniaPs2NewChartStreamReader : IBeatmaniaPs2NewChartStreamReader
{
    public BeatmaniaPs2Chart Read(Stream stream, long length)
    {
        Span<byte> buffer = stackalloc byte[8];
        if (length < 8)
            throw new RhythmCodexException("Invalid chart length.");

        stream.ReadExactly(buffer);
        var offset = 8;

        if (buffer.AsS32L() != 8)
            throw new RhythmCodexException("Invalid format identifier.");

        var rate = new BigRational(buffer[4..].AsS32L(), 1000000);
        var events = new List<BeatmaniaPs2Event>();
        var noteCounts = new Dictionary<int, int>();

        while (offset < length)
        {
            stream.ReadExactly(buffer[..4]);

            var linearTime = buffer.AsS32L();

            if (linearTime == 0x7FFF)
                break;

            stream.ReadExactly(buffer[4..]);
            offset += 8;

            var command = (BeatmaniaPs2EventType)(buffer[4] & 0xF);
            var param = buffer[4] >> 4;
            var value = (int)buffer[6..].AsU16L();

            if (linearTime == 0 && (int)command is 0 or 1)
            {
                noteCounts[(int)command] = noteCounts.GetValueOrDefault((int)command) + buffer[5];
                continue;
            }

            switch (command)
            {
                case BeatmaniaPs2EventType.Tempo:
                    value = buffer[5] | (param << 8);
                    param = 0;
                    break;
                case BeatmaniaPs2EventType.Meter:
                    value = buffer[5];
                    break;
                case BeatmaniaPs2EventType.Judgement:
                    value = unchecked((sbyte)buffer[5]);
                    break;
            }

            events.Add(new BeatmaniaPs2Event
            {
                LinearOffset = linearTime,
                Type = command,
                Parameter = param,
                Value = value
            });
        }

        return new BeatmaniaPs2Chart
        {
            Rate = rate,
            Events = events,
            NoteCounts = noteCounts
        };
    }
}