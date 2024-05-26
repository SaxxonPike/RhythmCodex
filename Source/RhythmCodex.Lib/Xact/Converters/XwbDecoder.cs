using System;
using System.Linq;
using RhythmCodex.Extensions;
using RhythmCodex.ImaAdpcm.Converters;
using RhythmCodex.ImaAdpcm.Models;
using RhythmCodex.IoC;
using RhythmCodex.Meta.Models;
using RhythmCodex.Sounds.Models;
using RhythmCodex.Wav.Converters;
using RhythmCodex.Wav.Models;
using RhythmCodex.Xact.Model;

namespace RhythmCodex.Xact.Converters;

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
                }).SingleOrDefault();
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