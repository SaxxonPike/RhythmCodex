using System;
using System.Collections.Generic;
using System.Linq;
using RhythmCodex.Archs.Djmain.Model;
using RhythmCodex.Games.Beatmania.Converters;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;
using RhythmCodex.Metadatas.Models;
using RhythmCodex.Sounds.Converters;
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

    private static void ApplyEffects(Sound sound)
    {
        sound.EnsureStereo();

        var leftData = sound.Samples[0].Data.ToArray();
        var rightData = sound.Samples[1].Data.ToArray();

        var volVal = (int)(sound[NumericData.SourceVolume] ?? 0) switch
        {
            <= 0x00 => 1,
            >= 0x7F => 0,
            var x => DjmainConstants.VolumeRom.Span[x] / (double)0x7FFF
        };

        var panIdx = Math.Clamp(((int)(sound[NumericData.SourcePanning] ?? 0x8) & 0xF) - 1, 0x0, 0xE);

        var leftVolVal = DjmainConstants.PanRom.Span[panIdx] / (double)0x7FFF;
        var rightVolVal = DjmainConstants.PanRom.Span[0xE - panIdx] / (double)0x7FFF;

        AudioSimd.Gain(leftData, (float)(leftVolVal * volVal));
        AudioSimd.Gain(rightData, (float)(rightVolVal * volVal));

        sound.Samples[0].Data = leftData;
        sound.Samples[1].Data = rightData;
        sound.Samples[0][NumericData.Volume] = null;
        sound.Samples[1][NumericData.Volume] = null;
        sound.Samples[0][NumericData.Panning] = null;
        sound.Samples[1][NumericData.Panning] = null;
        sound[NumericData.Volume] = null;
        sound[NumericData.Panning] = null;
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
                [NumericData.Channel] = (info.Channel & 0xF0) != 0 ? null : info.Channel,
                [NumericData.SourceChannel] = info.Channel,
                [NumericData.Rate] = beatmaniaDspTranslator.GetDjmainRate(info.Frequency),
                [NumericData.Id] = def.Key,
                ApplyEffectsHandler = ApplyEffects
            };
        }
    }
}