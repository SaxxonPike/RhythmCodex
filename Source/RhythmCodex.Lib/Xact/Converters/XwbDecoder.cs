using System.Collections.Generic;
using System.IO;
using System.Linq;
using RhythmCodex.Attributes;
using RhythmCodex.Extensions;
using RhythmCodex.ImaAdpcm.Converters;
using RhythmCodex.ImaAdpcm.Models;
using RhythmCodex.Infrastructure;
using RhythmCodex.Infrastructure.Models;
using RhythmCodex.IoC;
using RhythmCodex.Wav.Converters;
using RhythmCodex.Wav.Models;
using RhythmCodex.Xact.Model;

namespace RhythmCodex.Xact.Converters
{
    [Service]
    public class XwbDecoder : IXwbDecoder
    {
        private readonly IPcmDecoder _pcmDecoder;
        private readonly IImaAdpcmDecoder _imaAdpcmDecoder;
        private readonly IMicrosoftAdpcmDecoder _microsoftAdpcmDecoder;

        public XwbDecoder(
            IPcmDecoder pcmDecoder,
            IImaAdpcmDecoder imaAdpcmDecoder,
            IMicrosoftAdpcmDecoder microsoftAdpcmDecoder)
        {
            _pcmDecoder = pcmDecoder;
            _imaAdpcmDecoder = imaAdpcmDecoder;
            _microsoftAdpcmDecoder = microsoftAdpcmDecoder;
        }

        private ISound DecodeSound(XwbSound sound)
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
                            decoded = _pcmDecoder.Decode8Bit(sound.Data);
                            break;
                        case 16:
                            decoded = _pcmDecoder.Decode16Bit(sound.Data);
                            break;
                        case 24:
                            decoded = _pcmDecoder.Decode24Bit(sound.Data);
                            break;
                        case 32:
                            decoded = _pcmDecoder.Decode32Bit(sound.Data);
                            break;
                        default:
                            return null;
                    }

                    foreach (var channel in decoded.Deinterleave(1, format.Channels))
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
                    return _imaAdpcmDecoder.Decode(new ImaAdpcmChunk
                    {
                        Channels = format.Channels,
                        ChannelSamplesPerFrame = format.AdpcmSamplesPerBlock,
                        Data = sound.Data,
                        Rate = format.SampleRate
                    }).SingleOrDefault();
                }
                case XwbConstants.WavebankminiformatTagAdpcm:
                {
                    return _microsoftAdpcmDecoder.Decode(
                        sound.Data,
                        format,
                        new MicrosoftAdpcmFormat
                        {
                            Coefficients = new int[0],
                            SamplesPerBlock = format.AdpcmSamplesPerBlock
                        });
                }
                default:
                {
                    return null;
                }
            }
        }

        public ISound Decode(XwbSound sound)
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
}