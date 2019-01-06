using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using RhythmCodex.Attributes;
using RhythmCodex.Extensions;
using RhythmCodex.ImaAdpcm.Converters;
using RhythmCodex.ImaAdpcm.Models;
using RhythmCodex.Infrastructure;
using RhythmCodex.Infrastructure.Models;
using RhythmCodex.Riff.Streamers;
using RhythmCodex.Wav.Models;

namespace RhythmCodex.Wav.Converters
{
    [Service]
    public class WavDecoder : IWavDecoder
    {
        private readonly IRiffStreamReader _riffStreamReader;
        private readonly IPcmDecoder _pcmDecoder;
        private readonly IWaveFmtDecoder _waveFmtDecoder;
        private readonly IImaAdpcmDecoder _imaAdpcmDecoder;
        private readonly IMicrosoftAdpcmDecoder _microsoftAdpcmDecoder;

        public WavDecoder(
            IRiffStreamReader riffStreamReader, 
            IPcmDecoder pcmDecoder, 
            IWaveFmtDecoder waveFmtDecoder,
            IImaAdpcmDecoder imaAdpcmDecoder,
            IMicrosoftAdpcmDecoder microsoftAdpcmDecoder)
        {
            _riffStreamReader = riffStreamReader;
            _pcmDecoder = pcmDecoder;
            _waveFmtDecoder = waveFmtDecoder;
            _imaAdpcmDecoder = imaAdpcmDecoder;
            _microsoftAdpcmDecoder = microsoftAdpcmDecoder;
        }

        public ISound Decode(Stream stream)
        {
            var riff = _riffStreamReader.Read(stream);
            if (riff.Format != "WAVE")
                throw new RhythmCodexException("RIFF type must be WAVE.");

            var fmt = riff.Chunks.FirstOrDefault(c => c.Id == "fmt ");
            if (fmt == null)
                throw new RhythmCodexException("RIFF must contain the fmt chunk.");
            var format = _waveFmtDecoder.Decode(fmt);

            var data = riff.Chunks.FirstOrDefault(c => c.Id == "data");
            if (data == null)
                throw new RhythmCodexException("RIFF must contain the data chunk.");

            var result = new Sound
            {
                [NumericData.Rate] = format.SampleRate,
                Samples = new List<ISample>()
            };

            switch (format.Format)
            {
                case 0x0001: // raw PCM
                {
                    float[] decoded;
                    switch (format.BitsPerSample)
                    {
                        case 8:
                            decoded = _pcmDecoder.Decode8Bit(data.Data);
                            break;
                        case 16:
                            decoded = _pcmDecoder.Decode16Bit(data.Data);
                            break;
                        case 24:
                            decoded = _pcmDecoder.Decode24Bit(data.Data);
                            break;
                        case 32:
                            decoded = _pcmDecoder.Decode32Bit(data.Data);
                            break;
                        default:
                            throw new RhythmCodexException("Invalid bits per sample.");
                    }

                    foreach (var channel in decoded.Deinterleave(1, format.Channels))
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
                    var exFormat = new MicrosoftAdpcmFormat(format.ExtraData);
                    var decoded = _microsoftAdpcmDecoder.Decode(data.Data.AsSpan(), format, exFormat);

                    foreach (var sample in decoded.Samples)
                        result.Samples.Add(sample);

                    break;
                }
                case 0x0003: // 32-bit float
                {
                    var decoded = _pcmDecoder.DecodeFloat(data.Data);

                    foreach (var channel in decoded.Deinterleave(1, format.Channels))
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
                    var exFormat = new ImaAdpcmFormat(format.ExtraData);
                    var decoded = _imaAdpcmDecoder.Decode(new ImaAdpcmChunk
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
}