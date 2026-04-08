using System;
using System.Buffers;
using System.IO;
using RhythmCodex.Extensions;
using RhythmCodex.Games.Beatmania.Ps2.Models;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;
using RhythmCodex.Sounds.Vag.Streamers;
using RhythmCodex.Utils.Cursors;

namespace RhythmCodex.Games.Beatmania.Ps2.Streamers;

[Service]
public class BeatmaniaPs2OldBgmStreamReader(IVagStreamReader vagStreamReader)
    : IBeatmaniaPs2OldBgmStreamReader
{
    public BeatmaniaPs2Bgm? Read(Stream stream)
    {
        //
        // Read the header.
        //

        Span<byte> header = stackalloc byte[32];
        stream.ReadExactly(header);

        var length = header.AsS32B();
        var volume = header[5];
        var rate = header[6..].AsU16B();
        var channels = header[8];

        if (length <= 0 || channels <= 0)
            return null;

        stream.SkipBytes(0x800 - header.Length);

        //
        // Read the audio data.
        //

        using var mem = stream.ReadIntoTempStream(length);
        var data = vagStreamReader.Read(mem, channels, 0x800);

        return new BeatmaniaPs2Bgm
        {
            Data = data,
            Volume = volume,
            VolumeScale = 127,
            Rate = rate,
            Index = 1
        };
    }
}