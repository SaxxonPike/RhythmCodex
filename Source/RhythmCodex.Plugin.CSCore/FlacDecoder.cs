using System;
using System.IO;
using CSCore.Codecs.FLAC;
using RhythmCodex.IoC;
using RhythmCodex.Metadatas.Models;
using RhythmCodex.Plugin.CSCore.Lib.Codecs.FLAC;
using RhythmCodex.Sounds.Converters;
using RhythmCodex.Sounds.Flac.Converters;
using RhythmCodex.Sounds.Models;
using StreamExtensions = RhythmCodex.Infrastructure.StreamExtensions;

namespace RhythmCodex.Plugin.CSCore;

[Service]
public class FlacDecoder(IAudioDsp audioDsp) : IFlacDecoder
{
    public Sound? Decode(Stream stream)
    {
        using var inputStream = new FlacFile(stream);
        var samples = StreamExtensions.ReadAllBytes((StreamExtensions.SpanReadFunc)inputStream.Read);

        var result = audioDsp.BytesToSound(
            samples.Span,
            inputStream.WaveFormat.BitsPerSample,
            inputStream.WaveFormat.Channels,
            false
        );

        if (result == null)
            return null;

        result[NumericData.Rate] = inputStream.WaveFormat.SampleRate;
        return result;
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