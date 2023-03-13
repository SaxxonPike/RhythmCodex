using System.Collections.Generic;
using System.IO;
using System.Linq;
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
            throw new RhythmCodexException($"Channel count must be at least 1.");
        if (interleave != 0 && interleave < 16)
            throw new RhythmCodexException($"Interleave must be at least 16.");
        if ((interleave & 0xF) != 0)
            throw new RhythmCodexException($"Interleave must be a multiple of 16.");
        if (interleave == 0)
            interleave = 16;
            
        return new VagChunk
        {
            Data = ReadInternal(stream, channels, interleave).ToArray(),
            Channels = channels,
            Interleave = interleave
        };
    }

    private IEnumerable<byte> ReadInternal(Stream stream, int channels, int interleave)
    {
        var ended = false;
        var buffer = new byte[0x10];

        while (!ended)
        {
            for (var c = 0; c < channels; c++)
            {
                for (var i = 0; i < interleave; i += 16)
                {
                    var toRead = 16;
                    while (toRead > 0)
                    {
                        var justRead = stream.Read(buffer, 0, 0x10);
                        if (justRead == 0)
                            yield break;
                        toRead -= justRead;
                    }
                    
                    if ((buffer[1] & 0x01) == 0x01)
                        ended = true;

                    for (var j = 0; j < 16; j++)
                        yield return buffer[j];
                }                    
            }
        }
    }
}