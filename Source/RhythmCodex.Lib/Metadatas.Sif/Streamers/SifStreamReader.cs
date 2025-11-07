using System;
using System.IO;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;
using RhythmCodex.Metadatas.Sif.Converters;
using RhythmCodex.Metadatas.Sif.Models;

namespace RhythmCodex.Metadatas.Sif.Streamers;

[Service]
public class SifStreamReader(
    IBinarySifDecoder binarySifDecoder,
    ITextSifDecoder textSifDecoder)
    : ISifStreamReader
{
    public SifInfo Read(Stream stream, long length)
    {
        var data = new BinaryReader(stream).ReadBytes((int) length);
        return data[0] == 0x00 
            ? binarySifDecoder.Decode(data) 
            : ReadTextSif(data);
    }

    private SifInfo ReadTextSif(ReadOnlyMemory<byte> data)
    {
        using var dataStream = new ReadOnlyMemoryStream(data);
        return textSifDecoder.Decode(dataStream.ReadAllLines());
    }
}