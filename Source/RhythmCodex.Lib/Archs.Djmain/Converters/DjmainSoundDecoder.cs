using System;
using System.Collections.Generic;
using System.Linq;
using RhythmCodex.Archs.Djmain.Model;
using RhythmCodex.Games.Beatmania.Converters;
using RhythmCodex.IoC;
using RhythmCodex.Metadatas.Models;
using RhythmCodex.Sounds.Models;

namespace RhythmCodex.Archs.Djmain.Converters;

[Service]
public class DjmainSoundDecoder(IDjmainAudioDecoder djmainAudioDecoder, IBeatmaniaDspTranslator beatmaniaDspTranslator)
    : IDjmainSoundDecoder
{
    public Dictionary<int, Sound> Decode(IEnumerable<KeyValuePair<int, DjmainSample>> samples)
    {
        return DecodeInternal(samples)
            .ToDictionary(s => (int)s[NumericData.Id]!.Value, s => s);
    }

    private IEnumerable<Sound> DecodeInternal(IEnumerable<KeyValuePair<int, DjmainSample>> samples)
    {
        foreach (var def in samples)
        {
            var info = def.Value.Info;
            var data = def.Value.Data;

            var sample = new Sample();

            switch (def.Value.Info.SampleType & 0xC)
            {
                case 0x0:
                    sample.Data = djmainAudioDecoder.DecodePcm8(data.Span);
                    break;
                case 0x4:
                    sample.Data = djmainAudioDecoder.DecodePcm16(data.Span);
                    break;
                case 0x8:
                    sample.Data = djmainAudioDecoder.DecodeDpcm(data.Span);
                    break;
                default:
                    sample.Data = Memory<float>.Empty;
                    break;
            }

            yield return new Sound
            {
                Samples = [sample],
                [NumericData.Volume] = beatmaniaDspTranslator.GetDjmainVolume(info.Volume),
                [NumericData.SourceVolume] = info.Volume,
                [NumericData.Panning] = beatmaniaDspTranslator.GetDjmainPanning(info.Panning),
                [NumericData.SourcePanning] = info.Panning,
                [NumericData.Channel] = info.Channel,
                [NumericData.Rate] = beatmaniaDspTranslator.GetDjmainRate(info.Frequency),
                [NumericData.Id] = def.Key
            };
        }
    }
}