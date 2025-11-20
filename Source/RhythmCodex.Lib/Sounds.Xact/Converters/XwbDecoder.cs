using System;
using RhythmCodex.Extensions;
using RhythmCodex.IoC;
using RhythmCodex.Metadatas.Models;
using RhythmCodex.Sounds.ImaAdpcm.Converters;
using RhythmCodex.Sounds.ImaAdpcm.Models;
using RhythmCodex.Sounds.Models;
using RhythmCodex.Sounds.Wav.Converters;
using RhythmCodex.Sounds.Wav.Models;
using RhythmCodex.Sounds.Xact.Model;

namespace RhythmCodex.Sounds.Xact.Converters;

[Service]
public class XwbDecoder(
    IPcmDecoder pcmDecoder,
    IImaAdpcmDecoder imaAdpcmDecoder,
    IMicrosoftAdpcmDecoder microsoftAdpcmDecoder)
    : IXwbDecoder
{
    private Sound? DecodeSound(XwbSound sound)
    {
        var result = new Sound();
        var format = sound.Info.Format;

        switch (format.FormatTag)
        {
            case XwbConstants.WavebankminiformatTagPcm:
            {
                float[] decoded;
                switch (format.BitsPerSample)
                {
                    case 8:
                        decoded = pcmDecoder.Decode8Bit(sound.Data.Span);
                        break;
                    case 16:
                        decoded = pcmDecoder.Decode16Bit(sound.Data.Span);
                        break;
                    case 24:
                        decoded = pcmDecoder.Decode24Bit(sound.Data.Span);
                        break;
                    case 32:
                        decoded = pcmDecoder.Decode32Bit(sound.Data.Span);
                        break;
                    default:
                        return null;
                }

                foreach (var channel in decoded.AsSpan().Deinterleave(1, format.Channels))
                {
                    result.Samples.Add(new Sample
                    {
                        [NumericData.Rate] = format.SampleRate,
                        Data = channel
                    });
                }

                return result;
            }
            case XwbConstants.WavebankminiformatTagXma:
            {
                return imaAdpcmDecoder.Decode(new ImaAdpcmChunk
                {
                    Channels = format.Channels,
                    ChannelSamplesPerFrame = format.AdpcmSamplesPerBlock,
                    Data = sound.Data,
                    Rate = format.SampleRate
                });
            }
            case XwbConstants.WavebankminiformatTagAdpcm:
            {
                return microsoftAdpcmDecoder.Decode(
                    sound.Data.Span,
                    format,
                    new MicrosoftAdpcmFormat
                    {
                        Coefficients = [],
                        SamplesPerBlock = format.AdpcmSamplesPerBlock
                    });
            }
            default:
            {
                return null;
            }
        }
    }

    public Sound? Decode(XwbSound sound)
    {
        var result = DecodeSound(sound);
        if (result != null)
        {
            result[NumericData.Rate] = sound.Info.Format.SampleRate;
            result[StringData.Name] = sound.Name;
            result[NumericData.LoopStart] = sound.Info.LoopRegion.StartSample;
            result[NumericData.LoopLength] = sound.Info.LoopRegion.TotalSamples;
        }

        return result;
    }
}