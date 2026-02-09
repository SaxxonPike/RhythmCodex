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
public class DjmainSoundDecoder(
    IDjmainAudioDecoder djmainAudioDecoder, 
    IBeatmaniaDspTranslator beatmaniaDspTranslator,
    IDjmainMixer djmainMixer
) : IDjmainSoundDecoder
{
    public Dictionary<int, Sound> Decode(IEnumerable<KeyValuePair<int, DjmainSample>> samples, bool swapStereo)
    {
        return DecodeInternal(samples, swapStereo)
            .ToDictionary(s => (int)s[NumericData.Id]!.Value, s => s);
    }

    private IEnumerable<Sound> DecodeInternal(IEnumerable<KeyValuePair<int, DjmainSample>> samples, bool swapStereo)
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

            var result = new Sound
            {
                Samples = [sample],
                Mixer = () => djmainMixer,
                [NumericData.Volume] = beatmaniaDspTranslator.GetDjmainVolume(info.Volume),
                [NumericData.SourceVolume] = info.Volume,
                [NumericData.Panning] = beatmaniaDspTranslator.GetDjmainPanning(info.Panning, swapStereo),
                [NumericData.SourcePanning] = info.Panning,
                [NumericData.SourceChannel] = info.Channel,
                [NumericData.Rate] = beatmaniaDspTranslator.GetDjmainRate(info.Frequency),
                [NumericData.Id] = def.Key
            };

            //
            // Channels 0x00-0x07 are explicitly played on a single channel.
            //

            if (info.Channel < 0x08)
                result[NumericData.Channel] = info.Channel;

            //
            // Channels 0x10-0x1F indicate a priority system instead.
            //

            else if (info.Channel is >= 0x10 and <= 0x1F)
                result[NumericData.Priority] = 0x20 - info.Channel;
            
            //
            // Channel 0x20 indicates a deleted/muted sound.
            //
            
            yield return result;
        }
    }
}