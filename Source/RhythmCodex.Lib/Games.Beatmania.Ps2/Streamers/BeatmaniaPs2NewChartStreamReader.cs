using System;
using System.Collections.Generic;
using System.IO;
using RhythmCodex.Compressions.BemaniLz.Processors;
using RhythmCodex.Games.Beatmania.Ps2.Models;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;
using RhythmCodex.Utils.Cursors;

namespace RhythmCodex.Games.Beatmania.Ps2.Streamers;

[Service]
public sealed class BeatmaniaPs2NewChartStreamReader(IBemaniLzDecoder bemaniLzDecoder)
    : IBeatmaniaPs2NewChartStreamReader
{
    public BeatmaniaPs2Chart Read(Stream stream, long length)
    {
        Span<byte> buffer = stackalloc byte[8];
        if (length < 8)
            throw new RhythmCodexException("Invalid chart length.");

        stream.ReadExactly(buffer);
        var offset = 8;

        //
        // Charts may be compressed. In this case, we read in the remainder of the file data,
        // decompress it, then advance the position in the new decompressed stream accordingly.
        // This presupposes some things about the LZ format - the first byte of the file will
        // always be found in the second byte of the compressed output for these charts.
        //

        var actualStream = stream;
        var actualLength = length;

        if (buffer[0] != 0 && buffer[1] == 8)
        {
            var decodeStream = new MemoryStream((int)length);
            decodeStream.Write(buffer);
            stream.CopyTo(decodeStream);
            decodeStream.Position = 0;
            actualStream = new ReadOnlyMemoryStream(bemaniLzDecoder.Decode(decodeStream));
            actualLength = actualStream.Length;
            actualStream.ReadExactly(buffer);
        }

        //
        // Check the identifier. These charts always seem to start with 08 00 00 00.
        //

        if (buffer.AsS32L() != 8)
            throw new RhythmCodexException("Invalid format identifier.");

        var events = new List<BeatmaniaPs2Event>();
        var noteCounts = new Dictionary<int, int>();

        //
        // This chart format contains the "tick rate" in microseconds. I suspect some of the original
        // charts had some rounding error in the rate calculation, so we bias the rate a little faster
        // to compensate.
        //

        var rateValue = buffer[4..].AsS32L() * 2 - 1;
        var rate = new BigRational(rateValue, TimeSpan.MicrosecondsPerSecond * 2);

        //
        // Convert each chart event.
        //

        while (offset < actualLength)
        {
            //
            // The end indicator may only be 4 bytes, so only read 4 bytes at a time.
            //

            actualStream.ReadExactly(buffer[..4]);

            var linearTime = buffer.AsS32L();

            if (linearTime == 0x7FFF)
                break;

            actualStream.ReadExactly(buffer[4..]);
            offset += 8;

            //
            // Convert the chart event.
            //

            var command = (BeatmaniaPs2EventType)(buffer[4] & 0xF);
            var param = buffer[4] >> 4;
            var value = (int)buffer[6..].AsU16L();

            if (linearTime == 0 && (int)command is 0 or 1)
            {
                noteCounts[(int)command] = noteCounts.GetValueOrDefault((int)command) + buffer[5];
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
                    value = buffer[5] | (param << 8);
                    param = 0;
                    break;

                case BeatmaniaPs2EventType.Meter:
                    value = buffer[5];
                    break;

                //
                // Judgement events indicate the number of frames for the timing window. These
                // values are signed.
                //

                case BeatmaniaPs2EventType.Judgement:
                    value = unchecked((sbyte)buffer[5]);
                    break;
                
                //
                // BGM sound ID 1 plays the streamed BGM.
                //

                case BeatmaniaPs2EventType.Bgm:
                    if (value == 1)
                        value = 0;
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