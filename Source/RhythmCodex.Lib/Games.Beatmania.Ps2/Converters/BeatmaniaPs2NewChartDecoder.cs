using System;
using System.Collections.Generic;
using RhythmCodex.Games.Beatmania.Ps2.Models;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;
using RhythmCodex.Utils.Cursors;

namespace RhythmCodex.Games.Beatmania.Ps2.Converters;

/// <inheritdoc />
[Service]
public sealed class BeatmaniaPs2NewChartDecoder : IBeatmaniaPs2NewChartDecoder
{
    /// <inheritdoc />
    public BeatmaniaPs2Chart Decode(ReadOnlySpan<byte> data)
    {
        //
        // The "new" format consists of 8 bytes per event:
        //
        // bits 0...31: linear time (frames)
        // bits 32..35: command
        // bits 36..39: parameter[0]
        // bits 40..47: parameter[1]
        // bits 48..63: value
        //
        // Events will use either parameter[0] or parameter[1] but not both.
        // I assume this is an artifact of the conversion process that was
        // used to create the game files.
        //

        var events = new List<BeatmaniaPs2Event>();
        var noteCounts = new Dictionary<int, int>();

        //
        // This chart format contains the "tick rate" in microseconds. I suspect some of the original
        // charts had some rounding error in the rate calculation, so we bias the rate a little faster
        // to compensate. There is no real method to determine the correctness of this approach; it is
        // entirely subjective. I do not currently know precisely how the game calculates the rate
        // conversion to actual display frames.
        //

        var rateValue = data[4..].AsS32L() * 2 - 1;
        var rate = new BigRational(rateValue, TimeSpan.MicrosecondsPerSecond * 2);

        //
        // Convert each chart event.
        //

        var offset = 8;
        var cursor = data;
        var playedBgm = false;

        while (cursor.Length >= 8)
        {
            var skip = false;
            var linearTime = cursor.AsS32L();

            //
            // End of file is marked with FF 7F 00 00.
            //

            if (linearTime == 0x7FFF)
                break;

            //
            // Convert the chart event.
            //

            var command = (BeatmaniaPs2EventType)(cursor[4] & 0xF);
            var param = cursor[4] >> 4;
            var value = (int)cursor[6..].AsU16L();

            //
            // A "note count" precedes actual chart data. These look like
            // regular events but are not. The values are also split into
            // groups of 250, so multiple consecutive counts are combined.
            //

            if (linearTime == 0 && (int)command is 0 or 1)
            {
                noteCounts[(int)command] = noteCounts.GetValueOrDefault((int)command) + cursor[5];
                cursor = cursor[8..];
                continue;
            }

            //
            // Certain events put their information in a byte that is separate to the value
            // word, so we move that information to the value word.
            //

            switch (command)
            {
                //
                // BPM has bits 8..11 encoded in the parameter.
                //

                case BeatmaniaPs2EventType.Tempo:
                    value = cursor[5] | (param << 8);
                    param = 0;
                    break;

                //
                // It is uncertain what meter is used for, but it is encoded in some charts.
                //

                case BeatmaniaPs2EventType.Meter:
                    value = cursor[5];
                    break;

                //
                // BGM sound ID 1 plays the streamed BGM. Only allow the streamed BGM once.
                //

                case BeatmaniaPs2EventType.Bgm when value == 1:
                    if (!playedBgm)
                    {
                        playedBgm = true;
                        value = 0;
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
                    value = unchecked((sbyte)cursor[5]);
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

            cursor = cursor[8..];
        }

        return new BeatmaniaPs2Chart
        {
            Rate = rate,
            Events = events,
            NoteCounts = noteCounts,
            SpeedMult = 1
        };
    }
}