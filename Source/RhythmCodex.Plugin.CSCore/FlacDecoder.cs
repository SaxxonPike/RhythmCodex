using System;
using System.IO;
using System.Linq;
using CSCore.Codecs.FLAC;
using RhythmCodex.Extensions;
using RhythmCodex.IoC;
using RhythmCodex.Metadatas.Models;
using RhythmCodex.Plugin.CSCore.Lib.Codecs.FLAC;
using RhythmCodex.Sounds.Flac.Converters;
using RhythmCodex.Sounds.Models;
using RhythmCodex.Utils.Cursors;
using StreamExtensions = RhythmCodex.Infrastructure.StreamExtensions;

namespace RhythmCodex.Plugin.CSCore;

[Service]
public class FlacDecoder : IFlacDecoder
{
    public Sound Decode(Stream stream)
    {
        using var inputStream = new FlacFile(stream);
        var samples = StreamExtensions.ReadAllBytes(inputStream.Read)
            .Span
            .Deinterleave(2, inputStream.WaveFormat.Channels)
            .Select(bytes => new Sample
            {
                Data = bytes.CastU16L().Select(s => s / 32768f)
            })
            .ToList();
                
        return new Sound
        {
            Samples = samples,
            [NumericData.Rate] = inputStream.WaveFormat.SampleRate
        };
    }

    public Memory<byte> DecodeFrame(Stream stream, int blockSize)
    {
        var frame = FlacFrame.FromStream(stream, new FlacMetadataStreamInfo
        {
            SampleRate = 44100,
            Channels = 2,
            BitsPerSample = 16
        });
        Memory<byte> buffer = null;
        frame.NextFrame();
        frame.GetBuffer(ref buffer);
        return buffer;
    }
}