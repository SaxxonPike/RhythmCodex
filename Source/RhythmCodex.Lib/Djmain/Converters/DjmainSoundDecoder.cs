using System;
using System.Collections.Generic;
using System.Linq;
using RhythmCodex.Attributes;
using RhythmCodex.Audio;
using RhythmCodex.Djmain.Model;
using RhythmCodex.Infrastructure;

namespace RhythmCodex.Djmain.Converters
{
    [Service]
    public class DjmainSoundDecoder : IDjmainSoundDecoder
    {
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

            return Enumerable.Range(0, 16).Select(i =>
                    BigRational.One - new BigRational(Math.Max(0, i - minimum), range))
                .ToArray();
        });

        private readonly IDjmainAudioDecoder _djmainAudioDecoder;

        public DjmainSoundDecoder(IDjmainAudioDecoder djmainAudioDecoder)
        {
            _djmainAudioDecoder = djmainAudioDecoder;
        }

        public IDictionary<int, ISound> Decode(IEnumerable<KeyValuePair<int, IDjmainSample>> samples)
        {
            return DecodeInternal(samples).ToDictionary(s => (int)s[NumericData.Id].Value, s => s);
        }

        private IEnumerable<ISound> DecodeInternal(IEnumerable<KeyValuePair<int, IDjmainSample>> samples)
        {
            foreach (var def in samples)
            {
                var info = def.Value.Info;
                var data = def.Value.Data;

                var sample = new Sample();

                switch (def.Value.Info.SampleType & 0xC)
                {
                    case 0x0:
                        sample.Data = _djmainAudioDecoder.DecodePcm8(data);
                        break;
                    case 0x4:
                        sample.Data = _djmainAudioDecoder.DecodePcm16(data);
                        break;
                    case 0x8:
                        sample.Data = _djmainAudioDecoder.DecodeDpcm(data);
                        break;
                    default:
                        sample.Data = new List<float>();
                        break;
                }

                yield return new Sound
                {
                    Samples = new List<ISample> {sample},
                    [NumericData.Volume] = VolumeTable.Value[info.Volume],
                    [NumericData.Panning] = PanningTable.Value[(info.Panning & 0xF)],
                    [NumericData.Rate] = DjmainConstants.SampleRateMultiplier * info.Frequency,
                    [NumericData.Id] = def.Key
                };
            }
        }
    }
}