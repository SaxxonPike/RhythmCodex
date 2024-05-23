using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using RhythmCodex.Extensions;
using RhythmCodex.ImaAdpcm.Converters;
using RhythmCodex.ImaAdpcm.Models;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;
using RhythmCodex.Meta.Models;
using RhythmCodex.Riff.Streamers;
using RhythmCodex.Sounds.Models;
using RhythmCodex.Wav.Models;

namespace RhythmCodex.Wav.Converters;

[Service]
public class WavDecoder(
    IRiffStreamReader riffStreamReader,
    IPcmDecoder pcmDecoder,
    IWaveFmtDecoder waveFmtDecoder,
    IImaAdpcmDecoder imaAdpcmDecoder,
    IMicrosoftAdpcmDecoder microsoftAdpcmDecoder)
    : IWavDecoder
{
    public Sound? Decode(Stream stream)
    {
        var riff = riffStreamReader.Read(stream);
        if (riff.Format != "WAVE")
            throw new RhythmCodexException("RIFF type must be WAVE.");

        var fmt = riff.Chunks.FirstOrDefault(c => c.Id == "fmt ");
        if (fmt == null)
            throw new RhythmCodexException("RIFF must contain the fmt chunk.");
        var format = waveFmtDecoder.Decode(fmt);

        var data = riff.Chunks.FirstOrDefault(c => c.Id == "data");
        if (data == null)
            throw new RhythmCodexException("RIFF must contain the data chunk.");

        var result = new Sound
        {
            [NumericData.Rate] = format.SampleRate,
            Samples = new List<Sample>()
        };

        switch (format.Format)
        {
            case 0x0001: // raw PCM
            {
                float[] decoded;
                switch (format.BitsPerSample)
                {
                    case 8:
                        decoded = pcmDecoder.Decode8Bit(data.Data);
                        break;
                    case 16:
                        decoded = pcmDecoder.Decode16Bit(data.Data);
                        break;
                    case 24:
                        decoded = pcmDecoder.Decode24Bit(data.Data);
                        break;
                    case 32:
                        decoded = pcmDecoder.Decode32Bit(data.Data);
                        break;
                    default:
                        throw new RhythmCodexException("Invalid bits per sample.");
                }

                foreach (var channel in decoded.AsSpan().Deinterleave(1, format.Channels))
                {
                    result.Samples.Add(new Sample
                    {
                        [NumericData.Rate] = format.SampleRate,
                        Data = channel
                    });
                }

                break;
            }
            case 0x0002: // Microsoft ADPCM
            {
                var exFormat = new MicrosoftAdpcmFormat(format.ExtraData.Span);
                var decoded = microsoftAdpcmDecoder.Decode(data.Data, format, exFormat);

                foreach (var sample in decoded.Samples)
                    result.Samples.Add(sample);

                break;
            }
            case 0x0003: // 32-bit float
            {
                var decoded = pcmDecoder.DecodeFloat(data.Data);

                foreach (var channel in decoded.AsSpan().Deinterleave(1, format.Channels))
                {
                    result.Samples.Add(new Sample
                    {
                        [NumericData.Rate] = format.SampleRate,
                        Data = channel
                    });
                }

                break;
            }
            case 0x0011: // IMA ADPCM
            {
                var exFormat = new ImaAdpcmFormat(format.ExtraData.Span);
                var decoded = imaAdpcmDecoder.Decode(new ImaAdpcmChunk
                {
                    Channels = format.Channels,
                    Rate = format.SampleRate,
                    Data = data.Data,
                    ChannelSamplesPerFrame = exFormat.SamplesPerBlock
                });

                foreach (var sample in decoded.SelectMany(s => s.Samples))
                    result.Samples.Add(sample);
                    
                break;
            }
        }

        return result;
    }
}