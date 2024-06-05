using System;
using System.IO;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;
using RhythmCodex.Vag.Models;

namespace RhythmCodex.Vag.Streamers;

[Service]
public class VagStreamReader : IVagStreamReader
{
    public VagChunk Read(Stream stream, int channels, int interleave)
    {
        if (channels < 1)
            throw new RhythmCodexException("Channel count must be at least 1.");
        if (interleave != 0 && interleave < 16)
            throw new RhythmCodexException("Interleave must be at least 16.");
        if ((interleave & 0xF) != 0)
            throw new RhythmCodexException("Interleave must be a multiple of 16.");
        if (interleave == 0)
            interleave = 16;

        var data = ReadInternal(stream, channels, interleave);

        return new VagChunk
        {
            Data = data.GetBuffer().AsMemory(0, (int)data.Length),
            Channels = channels,
            Interleave = interleave
        };
    }

    private static MemoryStream ReadInternal(Stream stream, int channels, int interleave)
    {
        var ended = false;
        var total = channels * interleave;
        var block = new byte[total];
        var output = new MemoryStream();

        while (!ended)
        {
            var cursor = block.AsSpan();
            stream.ReadAtLeast(cursor, total, false);

            for (var c = 0; c < channels; c++)
            {
                var blank = false;

                for (var i = 0; i < interleave; i += 0x10)
                {
                    if (blank)
                    {
                        cursor[..0x10].Clear();
                    }
                    else if ((cursor[1] & 0x01) == 0x01)
                    {
                        ended = true;
                        blank = true;
                    }

                    cursor = cursor[0x10..];
                }
            }
        }

        return output;
    }
}