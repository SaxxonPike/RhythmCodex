using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Numerics;
using RhythmCodex.Attributes;
using RhythmCodex.Audio;
using RhythmCodex.Djmain.Model;
using RhythmCodex.Djmain.Streamers;

namespace RhythmCodex.Djmain.Converters
{
    public class DjmainSoundDecoder
    {
        private readonly IAudioDecoder _audioDecoder;
        private readonly IAudioStreamReader _audioStreamReader;
        private readonly IDjmainConfiguration _djmainConfiguration;

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
            IAudioDecoder audioDecoder,
            IAudioStreamReader audioStreamReader,
            IDjmainConfiguration djmainConfiguration)
        {
            _audioDecoder = audioDecoder;
            _audioStreamReader = audioStreamReader;
            _djmainConfiguration = djmainConfiguration;
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
                        sample.Data = _audioDecoder.DecodePcm8(_audioStreamReader.ReadPcm8(stream));
                        break;
                    case 0x4:
                        sample.Data = _audioDecoder.DecodePcm16(_audioStreamReader.ReadPcm16(stream));
                        break;
                    case 0x8:
                        sample.Data = _audioDecoder.DecodeDpcm(_audioStreamReader.ReadDpcm(stream));
                        break;
                }

                sample[NumericData.Rate] = _djmainConfiguration.SampleRateMultiplier * def.Value.Frequency;

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
