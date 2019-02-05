using System;
using System.Collections.Generic;
using System.Linq;
using RhythmCodex.Attributes;
using RhythmCodex.Beatmania.Converters;
using RhythmCodex.Djmain.Model;
using RhythmCodex.Infrastructure;
using RhythmCodex.Infrastructure.Models;
using RhythmCodex.IoC;
using RhythmCodex.Riff;

namespace RhythmCodex.Djmain.Converters
{
    [Service]
    public class DjmainSoundDecoder : IDjmainSoundDecoder
    {
        private readonly IDjmainAudioDecoder _djmainAudioDecoder;
        private readonly IBeatmaniaDspTranslator _beatmaniaDspTranslator;

        public DjmainSoundDecoder(IDjmainAudioDecoder djmainAudioDecoder, IBeatmaniaDspTranslator beatmaniaDspTranslator)
        {
            _djmainAudioDecoder = djmainAudioDecoder;
            _beatmaniaDspTranslator = beatmaniaDspTranslator;
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
                    [NumericData.Volume] = _beatmaniaDspTranslator.GetDjmainVolume(info.Volume),
                    [NumericData.SourceVolume] = info.Volume,
                    [NumericData.Panning] = _beatmaniaDspTranslator.GetDjmainPanning(info.Panning),
                    [NumericData.SourcePanning] = info.Panning,
                    [NumericData.Channel] = info.Channel,
                    [NumericData.Rate] = _beatmaniaDspTranslator.GetDjmainRate(info.Frequency),
                    [NumericData.Id] = def.Key
                };
            }
        }
    }
}