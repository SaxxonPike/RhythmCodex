using System;
using System.Buffers;
using System.IO;
using RhythmCodex.Games.Beatmania.Ps2.Models;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;
using RhythmCodex.Sounds.Vag.Streamers;
using RhythmCodex.Utils.Cursors;

namespace RhythmCodex.Games.Beatmania.Ps2.Streamers;

[Service]
public class BeatmaniaPs2NewBgmStreamReader(IVagStreamReader vagStreamReader)
    : IBeatmaniaPs2NewBgmStreamReader
{
    public BeatmaniaPs2Bgm Read(Stream stream)
    {
        //
        // Read the header.
        //

        const int fieldCount = 12;
        const int headerSize = fieldCount * 4;

        Span<int> header = stackalloc int[fieldCount];
        Span<byte> buffer = stackalloc byte[headerSize];

        stream.ReadExactly(buffer);

        for (var i = 0; i < fieldCount; i++)
            header[i] = buffer[(i * 4)..].AsS32L();

        var headerLength = header[2];
        var dataLength = header[3];
        var loopStart = header[4];
        var loopEnd = header[5];
        var rate = header[6];
        var channels = header[7];
        var interleave = header[9];
        var volume = header[10];

        stream.SkipBytes(headerLength - headerSize);
        
        //
        // Read the audio data.
        //

        using var mem = stream.ReadIntoTempStream(dataLength);

        return new BeatmaniaPs2Bgm
        {
            Channels = channels,
            Rate = rate,
            Volume = volume,
            VolumeScale = 127,
            Data = vagStreamReader.Read(mem, channels, interleave),
            Index = 0
        };
    }
}