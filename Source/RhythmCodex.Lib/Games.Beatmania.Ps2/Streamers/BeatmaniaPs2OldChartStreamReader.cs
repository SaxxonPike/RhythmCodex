using System;
using System.IO;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;
using RhythmCodex.Utils.Cursors;

namespace RhythmCodex.Games.Beatmania.Ps2.Streamers;

[Service]
public sealed class BeatmaniaPs2OldChartStreamReader : IBeatmaniaPs2OldChartStreamReader
{
    public Memory<byte> Read(Stream stream, long length)
    {
        Span<byte> buffer = stackalloc byte[4];
        if (length < 4)
            throw new RhythmCodexException("Invalid chart length.");

        var offset = 0;
        var result = new MemoryStream();

        var actualStream = stream;
        var actualLength = length;

        //
        // Read until the end marker.
        //

        while (offset < actualLength)
        {
            actualStream.ReadExactly(buffer[..4]);

            var linearTime = buffer.AsS32L();
            result.Write(buffer);
            offset += 4;

            //
            // End of file is indicated with FF 7F 00 00.
            //

            if (linearTime == 0x7FFF) 
                break;
        }

        return result.ToArray();
    }
}