using System;
using System.IO;
using RhythmCodex.Extensions;
using RhythmCodex.Games.Beatmania.Ps2.Models;
using RhythmCodex.IoC;
using RhythmCodex.Sounds.Vag.Streamers;
using RhythmCodex.Utils.Cursors;

namespace RhythmCodex.Games.Beatmania.Ps2.Streamers;

[Service]
public class BeatmaniaPs2OldBgmStreamReader(IVagStreamReader vagStreamReader) : IBeatmaniaPs2OldBgmStreamReader
{
    public BeatmaniaPs2Bgm? Read(Stream stream)
    {
        var reader = new BinaryReader(stream);
        Span<byte> header = stackalloc byte[32];
        reader.ReadExactly(header);
        
        var length = header.AsS32B();
        var volume = header[5];
        var rate = header[6..].AsU16B();
        var channels = header[8];

        if (length < 0)
        {
            var flags0 = (length >> 28) & 0xF;
            length &= 0x0FFFFFFF;
        }

        if (channels <= 0)
        {
            return null;
        }

        // skip the rest of the header
        reader.Skip(0x800 - header.Length);

        var source = reader.ReadBytes(length);
        using var mem = new MemoryStream(source);
        var data = vagStreamReader.Read(mem, channels, 0x800);
        return new BeatmaniaPs2Bgm
        {
            Data = data,
            Volume = volume,
            VolumeScale = 80,
            Rate = rate,
            Index = 1
        };
    }
}