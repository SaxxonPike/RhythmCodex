using System;
using System.Linq;
using RhythmCodex.IoC;
using RhythmCodex.Meta.Models;
using RhythmCodex.Riff.Models;
using RhythmCodex.Sounds.Converters;
using RhythmCodex.Sounds.Models;

namespace RhythmCodex.Riff.Converters;

[Service]
public class RiffPcm16SoundEncoder(
    IRiffFormatEncoder formatEncoder,
    IAudioDsp audioDsp
) : IRiffPcm16SoundEncoder
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
            SampleRate = (int)sampleRate,
            Channels = channels,
            ByteRate = (int)byteRate!,
            BitsPerSample = 16,
            BlockAlign = channels * 2,
            ExtraData = Memory<byte>.Empty
        };

        container.Chunks.Add(formatEncoder.Encode(format));

        var mixed = audioDsp.Interleave16Bits(sound);

        container.Chunks.Add(new RiffChunk
        {
            Id = "data",
            Data = mixed
        });

        return container;
    }
}