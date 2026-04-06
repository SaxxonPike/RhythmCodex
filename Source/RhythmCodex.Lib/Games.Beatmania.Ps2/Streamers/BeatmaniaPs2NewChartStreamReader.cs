using System;
using System.IO;
using RhythmCodex.Compressions.BemaniLz.Processors;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;
using RhythmCodex.Utils.Cursors;

namespace RhythmCodex.Games.Beatmania.Ps2.Streamers;

[Service]
public sealed class BeatmaniaPs2NewChartStreamReader(IBemaniLzDecoder bemaniLzDecoder)
    : IBeatmaniaPs2NewChartStreamReader
{
    public Memory<byte> Read(Stream stream, long length)
    {
        if (length < 8)
            throw new RhythmCodexException("Invalid chart length.");

        Span<byte> buffer = stackalloc byte[8];
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

        var result = new MemoryStream();
        result.Write(buffer);

        //
        // Check the identifier. These charts always seem to start with 08 00 00 00.
        //

        if (buffer.AsS32L() != 8)
            throw new RhythmCodexException("Invalid format identifier.");

        //
        // Read until the end marker.
        //

        while (offset < actualLength)
        {
            //
            // The end indicator may only be 4 bytes, so only read 4 bytes at a time.
            //

            actualStream.ReadExactly(buffer[..4]);

            var linearTime = buffer.AsS32L();

            //
            // End of file is indicated with FF 7F 00 00.
            //

            if (linearTime == 0x7FFF)
            {
                buffer[4..].Clear();
                result.Write(buffer);
                break;
            }

            actualStream.ReadExactly(buffer[4..]);
            result.Write(buffer);
            offset += 8;
        }

        return result.ToArray();
    }
}