using System;
using System.Collections.Generic;
using RhythmCodex.Games.Beatmania.Ps2.Models;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;
using RhythmCodex.Utils.Cursors;

namespace RhythmCodex.Games.Beatmania.Ps2.Converters;

/// <inheritdoc />
[Service]
public sealed class BeatmaniaPs2OldChartDecoder : IBeatmaniaPs2OldChartDecoder
{
    /// <inheritdoc />
    public BeatmaniaPs2Chart Decode(ReadOnlySpan<byte> data)
    {
        //
        // The "old" format consists of 4 bytes per event:
        //
        // bits 0...15: linear time (frames)
        // bits 16..19: command
        // bits 20..23: parameter
        // bits 24..31: value
        //
        // Ordinarily the refresh rate of the game is synced to that
        // of an NTSC PlayStation GPU:
        // 9000000 / 572 / 263 = 59.82610545hz or 16.7151(...)ms.
        //
        // However, the team doing conversions for the "old" chart
        // style chose to use 16.716ms.
        //

        var rateValue = 16716;
        var cursor = data;
        var mult = 1d;

        //
        // It is possible starting with 5thstyle to specify a custom rate.
        // This should follow the same rules as "new" charts.
        // 

        if (data.AsS32L() == 8)
        {
            rateValue = data[4..].AsS32L();
            cursor = data[8..];
            mult = 1.00035d;
        }

        var rate = new BigRational(rateValue, TimeSpan.MicrosecondsPerSecond);

        var events = new List<BeatmaniaPs2Event>();
        var noteCounts = new Dictionary<int, int>();
        var playedBgm = false;
        var bgmStartTime = 0;

        //
        // Convert each chart event.
        //

        while (cursor.Length >= 4)
        {
            var skip = false;
            int linearTime = cursor.AsU16L();
            var command = (BeatmaniaPs2EventType)(byte)(cursor[2] & 0xF);
            var param = unchecked((byte)(cursor[2] >> 4));
            var value = (int)cursor[3];

            //
            // End of file is marked with FF 7F.
            //

            if (linearTime == 0x7FFF)
                break;

            //
            // A "note count" precedes actual chart data. These look like
            // regular events but are not. The values are also split into
            // groups of 250, so multiple consecutive counts are combined.
            //

            if (linearTime == 0 && (int)command is 0 or 1)
            {
                noteCounts[(int)command] = noteCounts.GetValueOrDefault((int)command) + value;
                cursor = cursor[4..];
                continue;
            }

            //
            // Certain events put their information in a byte that is separate to the value
            // byte, so we move that information to the value byte.
            //

            switch (command)
            {
                //
                // BPM has bits 8..11 encoded in the parameter.
                //

                case BeatmaniaPs2EventType.Tempo:
                    value |= param << 8;
                    param = 0;
                    break;

                //
                // In some source files, the main BGM appears multiple times. However, as it is
                // streamed, it is only ever played once. Therefore, we ignore further events
                // that attempt to replay the main BGM.
                //

                case BeatmaniaPs2EventType.Bgm when value <= 2:
                    if (!playedBgm)
                    {
                        playedBgm = true;
                        bgmStartTime = linearTime;
                    }
                    else
                    {
                        skip = true;
                    }
                    break;

                //
                // Judgement events indicate the number of frames for the timing window. These
                // values are signed.
                //

                case BeatmaniaPs2EventType.Judgement:
                    value = unchecked((sbyte)value);
                    break;
            }

            //
            // Commit the event and advance the pointer.
            //

            if (!skip)
            {
                events.Add(new BeatmaniaPs2Event
                {
                    LinearOffset = linearTime,
                    Type = command,
                    Parameter = param,
                    Value = value
                });
            }

            cursor = cursor[4..];
        }

        return new BeatmaniaPs2Chart
        {
            Rate = rate,
            Events = events,
            NoteCounts = noteCounts,
            SpeedMult = mult
        };
    }
}