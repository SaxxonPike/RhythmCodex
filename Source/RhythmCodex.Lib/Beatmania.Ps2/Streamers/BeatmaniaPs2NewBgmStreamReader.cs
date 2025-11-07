using System;
using System.IO;
using RhythmCodex.Beatmania.Ps2.Models;
using RhythmCodex.IoC;
using RhythmCodex.Vag.Streamers;

namespace RhythmCodex.Beatmania.Ps2.Streamers;

[Service]
public class BeatmaniaPs2NewBgmStreamReader(IVagStreamReader vagStreamReader) : IBeatmaniaPs2NewBgmStreamReader
{
    public BeatmaniaPs2Bgm Read(Stream stream)
    {
        const int fieldCount = 12;
        const int headerSize = fieldCount * 4;
            
        var reader = new BinaryReader(stream);
        Span<int> header = stackalloc int[fieldCount];
        for (var i = 0; i < fieldCount; i++)
            header[i] = reader.ReadInt32();

        var headerLength = header[2];
        var dataLength = header[3];
        var loopStart = header[4];
        var loopEnd = header[5];
        var rate = header[6];
        var channels = header[7];
        var interleave = header[9];
        var volume = header[10];

        reader.ReadBytes(headerLength - headerSize);

        var data = reader.ReadBytes(dataLength);
        using var mem = new MemoryStream(data);
        return new BeatmaniaPs2Bgm
        {
            Channels = channels,
            Rate = rate,
            Volume = volume,
            Data = vagStreamReader.Read(mem, channels, interleave)
        };
    }
}