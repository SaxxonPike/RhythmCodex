using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using Numerics;
using RhythmCodex.Attributes;
using RhythmCodex.Audio;
using RhythmCodex.Djmain.Model;
using RhythmCodex.Djmain.Streamers;

namespace RhythmCodex.Djmain.Converters
{
    public class DjmainSoundDecoder
    {
        private readonly IDpcmAudioDecoder _dpcmAudioDecoder;
        private readonly IPcm8AudioDecoder _pcm8AudioDecoder;
        private readonly IPcm16AudioDecoder _pcm16AudioDecoder;
        private readonly IDpcmAudioStreamReader _dpcmAudioStreamReader;
        private readonly IPcm8StreamReader _pcm8StreamReader;
        private readonly IPcm16StreamReader _pcm16StreamReader;

        private static readonly Lazy<BigRational[]> VolumeTable = new Lazy<BigRational[]>(() =>
        {
            const double referenceDecibels = -36d;
            const double referenceVolume = 0x40;
            const double attenuation = 20d;

            return Enumerable.Range(0, 256).Select(i => 
                new BigRational(Math.Pow(10d, referenceDecibels * i / referenceVolume / attenuation)))
                .ToArray();
        });

        private static readonly Lazy<BigRational[]> PanningTable = new Lazy<BigRational[]>(() =>
        {
            const int minimum = 0x1;
            const int range = 0xE;

            return Enumerable.Range(0, 15).Select(i =>
                BigRational.One - new BigRational(Math.Max(0, i - minimum), range))
                .ToArray();
        });

        public DjmainSoundDecoder(
            IDpcmAudioDecoder dpcmAudioDecoder,
            IPcm8AudioDecoder pcm8AudioDecoder,
            IPcm16AudioDecoder pcm16AudioDecoder,
            IDpcmAudioStreamReader dpcmAudioStreamReader,
            IPcm8StreamReader pcm8StreamReader,
            IPcm16StreamReader pcm16StreamReader)
        {
            _dpcmAudioDecoder = dpcmAudioDecoder;
            _pcm8AudioDecoder = pcm8AudioDecoder;
            _pcm16AudioDecoder = pcm16AudioDecoder;
            _dpcmAudioStreamReader = dpcmAudioStreamReader;
            _pcm8StreamReader = pcm8StreamReader;
            _pcm16StreamReader = pcm16StreamReader;
        }

        public IList<ISound> Decode(IEnumerable<KeyValuePair<int, DjmainSampleDefinition>> definitions, Stream stream, int offset)
        {
            return DecodeInternal(definitions, stream, offset).ToList();
        }

        private IEnumerable<ISound> DecodeInternal(IEnumerable<KeyValuePair<int, DjmainSampleDefinition>> definitions, Stream stream, int offset)
        {
            foreach (var def in definitions)
            {
                var sample = new Sample();

                stream.Position = (def.Value.Offset + offset) & 0xFFFFFF;

                switch (def.Value.SampleType & 0xC)
                {
                    case 0x0:
                        sample.Data = _pcm8AudioDecoder.Decode(_pcm8StreamReader.Read(stream));
                        break;
                    case 0x4:
                        sample.Data = _pcm16AudioDecoder.Decode(_pcm16StreamReader.Read(stream));
                        break;
                    case 0x8:
                        sample.Data = _dpcmAudioDecoder.Decode(_dpcmAudioStreamReader.Read(stream));
                        break;
                }

                sample[NumericData.Rate] = DjmainConstants.SampleRateMultiplier * def.Value.Frequency;

                yield return new Sound
                {
                    Samples = new List<ISample> { sample },
                    [NumericData.Volume] = VolumeTable.Value[def.Value.Volume],
                    [NumericData.Panning] = PanningTable.Value[def.Value.Panning & 0xF]
                };
            }
        }
    }
}
