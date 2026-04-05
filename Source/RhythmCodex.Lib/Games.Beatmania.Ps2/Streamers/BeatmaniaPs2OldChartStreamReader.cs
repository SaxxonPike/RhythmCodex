using System;
using System.Collections.Generic;
using System.IO;
using RhythmCodex.Games.Beatmania.Ps2.Models;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;
using RhythmCodex.Utils.Cursors;

namespace RhythmCodex.Games.Beatmania.Ps2.Streamers;

[Service]
public class BeatmaniaPs2OldChartStreamReader : IBeatmaniaPs2OldChartStreamReader
{
    public BeatmaniaPs2Chart Read(Stream stream, long length)
    {
        Span<byte> buffer = stackalloc byte[4];
        if (length < 4)
            throw new RhythmCodexException("Invalid chart length.");

        var offset = 0;

        //
        // Rate is tied to the non-interlaced video refresh rate of an NTSC PlayStation GPU:
        // 9000000 / 572 / 263 = 59.82610545hz or 16.7151(...)ms.
        //

        var rate = new BigRational(100000000L, 5982610545L);

        var events = new List<BeatmaniaPs2Event>();
        var noteCounts = new Dictionary<int, int>();
        var playedBgm = false;

        while (offset < length)
        {
            stream.ReadExactly(buffer);
            offset += 4;

            var linearTime = buffer.AsU16L();
            var command = (BeatmaniaPs2EventType)(byte)(buffer[2] & 0xF);
            var param = unchecked((byte)(buffer[2] >> 4));
            var value = (int)buffer[3];

            if (linearTime == 0x7FFF)
                break;

            if (linearTime == 0 && (int)command is 0 or 1)
            {
                noteCounts[(int)command] = noteCounts.GetValueOrDefault((int)command) + value;
                continue;
            }

            switch (command)
            {
                case BeatmaniaPs2EventType.Bgm when value == 1:
                    if (!playedBgm)
                        playedBgm = true;
                    else
                        value = 0;
                    break;
                case BeatmaniaPs2EventType.Tempo:
                    value |= param << 8;
                    param = 0;
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