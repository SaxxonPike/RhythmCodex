using System;
using System.IO;
using System.Linq;
using RhythmCodex.IoC;
using RhythmCodex.Meta.Models;
using RhythmCodex.Riff.Models;
using RhythmCodex.Sounds.Models;

namespace RhythmCodex.Riff.Converters;

[Service]
public class RiffPcm16SoundEncoder(IRiffFormatEncoder formatEncoder) : IRiffPcm16SoundEncoder
{
    public RiffContainer Encode(Sound? sound)
    {
        var sampleRate = sound[NumericData.Rate];

        if (sampleRate == null)
        {
            var sampleRates = sound
                .Samples
                .Select(s => s[NumericData.Rate])
                .Where(r => r != null)
                .Distinct()
                .ToArray();
            sampleRate = sampleRates.SingleOrDefault();
        }

        sampleRate ??= 44100;

        var channels = sound.Samples.Count;
        var byteRate = sampleRate * channels * 2;
        var container = new RiffContainer
        {
            Format = "WAVE",
            Chunks = []
        };

        var format = new RiffFormat
        {
            Format = 1,
            SampleRate = (int) sampleRate,
            Channels = channels,
            ByteRate = (int) byteRate!,
            BitsPerSample = 16,
            BlockAlign = channels * 2,
            ExtraData = Memory<byte>.Empty
        };

        container.Chunks.Add(formatEncoder.Encode(format));

        var totalSamples = sound.Samples.Max(s => s.Data.Length);

        using var stream = new MemoryStream();
        using var writer = new BinaryWriter(stream);
        for (var i = 0; i < totalSamples; i++)
        {
            for (var j = 0; j < sound.Samples.Count; j++)
            {
                var sample = sound.Samples[j];
                var source = sample.Data;
                var sourceValue = i < source.Length ? source.Span[i] : 0f;
                var value = Math.Round(sourceValue * 32767f);
                if (value > 32767f)
                    value = 32767f;
                else if (value < -32767f)
                    value = -32767f;
                writer.Write((short) value);
            }
        }

        container.Chunks.Add(new RiffChunk
        {
            Id = "data",
            Data = stream.ToArray()
        });

        return container;
    }
}