using System;
using System.Collections.Generic;
using System.IO;
using RhythmCodex.Extensions;
using RhythmCodex.IoC;

namespace RhythmCodex.Djmain.Streamers;

[Service]
public class DjmainAudioStreamWriter : IDjmainAudioStreamWriter
{
    public void WriteDpcm(Stream stream, ReadOnlySpan<byte> data)
    {
        var writer = new BinaryWriter(stream);
        writer.Write(data);
        writer.Write(DjmainConstants.DpcmEndMarker);
    }

    public void WritePcm8(Stream stream, ReadOnlySpan<byte> data)
    {
        var writer = new BinaryWriter(stream);
        writer.Write(data);
        writer.Write(DjmainConstants.Pcm8EndMarker);
    }

    public void WritePcm16(Stream stream, ReadOnlySpan<byte> data)
    {
        var writer = new BinaryWriter(stream);
        var arrayData = data;
        var length = arrayData.Length & ~1;
        stream.Write(arrayData[..length]);
        writer.Write(DjmainConstants.Pcm16EndMarker);
        writer.Write(DjmainConstants.Pcm16EndMarker);
    }
}