using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using RhythmCodex.Archs.Psx.Model;
using RhythmCodex.IoC;
using RhythmCodex.Utils.Cursors;

namespace RhythmCodex.Archs.Psx.Streamers;

[Service]
public sealed class PsxMgsSoundTableReader : IPsxMgsSoundTableReader
{
    public PsxMgsSoundTableBlock Read(Stream stream)
    {
        Span<byte> buffer = stackalloc byte[0x800];

        var header = new MemoryStream();
        var data = new MemoryStream();
        var scripts = new MemoryStream();

        //
        // Determine if any bytes are present in the header.
        //

        stream.ReadExactly(buffer[..16]);

        while (true)
        {
            if (buffer[4..].AsS32L() == 0 &&
                buffer[12..].AsS32L() == -1)
                break;

            header.Write(buffer[..16]);
            stream.ReadExactly(buffer[..16]);
        }

        //
        // Read the actual table.
        //

        var offsets = new List<int>();

        for (var i = 0; i < 128; i++)
        {
            offsets.Add(buffer[4..].AsS32L());
            offsets.Add(buffer[8..].AsS32L());
            offsets.Add(buffer[12..].AsS32L());
            data.Write(buffer[..16]);
            stream.ReadExactly(buffer[..16]);
        }

        //
        // Read the scripts.
        //

        var maxOffset = offsets.Max();

        if (maxOffset > 16)
        {
            var useStackBuffer = maxOffset <= buffer.Length;
            var skipBuffer = useStackBuffer ? buffer[..maxOffset] : new byte[maxOffset];
        
            if (!useStackBuffer)
                buffer[..16].CopyTo(skipBuffer);

            stream.ReadExactly(skipBuffer[16..]);
            scripts.Write(skipBuffer);
        }
        
        //
        // Read through the highest referenced script to determine the final size.
        //

        while (true)
        {
            stream.ReadExactly(buffer[..4]);
            scripts.Write(buffer[..4]);
            if (buffer[3] == 0xFF)
                break;
        }

        return new PsxMgsSoundTableBlock
        {
            Header = header.ToArray(),
            Table = data.ToArray(),
            Scripts = scripts.ToArray()
        };
    }
}