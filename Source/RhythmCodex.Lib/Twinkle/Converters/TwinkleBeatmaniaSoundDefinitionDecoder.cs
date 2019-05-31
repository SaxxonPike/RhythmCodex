using System;
using System.Collections.Generic;
using System.Linq;
using RhythmCodex.Beatmania.Providers;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;
using RhythmCodex.Meta.Models;
using RhythmCodex.Sounds.Models;
using RhythmCodex.Twinkle.Model;

namespace RhythmCodex.Twinkle.Converters
{
    [Service]
    public class TwinkleBeatmaniaSoundDefinitionDecoder : ITwinkleBeatmaniaSoundDefinitionDecoder
    {
        public TwinkleBeatmaniaSoundDefinition Decode(ReadOnlySpan<byte> data)
        {
            return new TwinkleBeatmaniaSoundDefinition
            {
                Channel = data[0x00],
                Flags01 = data[0x01],
                Frequency = Bitter.ToInt16S(data, 0x02) & 0xFFFF,
                Volume = data[0x04],
                Panning = data[0x05],
                SampleStart = Bitter.ToInt24S(data, 0x06) & 0xFFFFFF,
                SampleEnd = Bitter.ToInt24S(data, 0x09) & 0xFFFFFF,
                Value0C = Bitter.ToInt16S(data, 0x0C) & 0xFFFF,
                Flags0E = data[0x0E],
                Flags0F = data[0x0F],
                SizeInBlocks = Bitter.ToInt16S(data, 0x10) & 0xFFFF
            };
        }
    }

    [Service]
    public class TwinkleBeatmaniaSoundDecoder : ITwinkleBeatmaniaSoundDecoder
    {
        public ISound Decode(TwinkleBeatmaniaSoundDefinition definition, ReadOnlySpan<byte> data)
        {
            var offset = definition.SampleStart << 1;
            var stereo = (definition.Flags0F & 0x80) != 0;
            var length = (definition.SampleEnd - definition.SampleStart);
            var channels = Enumerable
                .Range(0, stereo ? 2 : 1)
                .Select(i => new List<float>(length >> (stereo ? 2 : 1)))
                .ToArray();
            var channelIndex = 0;
            var channelCount = channels.Length;
            
            for (var i = 0; i < length; i++)
            {
                var sample = (data[offset] << 8) | data[offset + 1];
                if (sample >= 0x8000)
                    sample = -(sample & 0x7FFF);

                channels[channelIndex].Add((float)sample / 0x7FFF);
                channelIndex++;
                while (channelIndex >= channelCount)
                    channelIndex -= channelCount;
                offset += 2;
            }
            
            return new Sound
            {
                [NumericData.Rate] = definition.Frequency,
                [NumericData.SourceRate] = definition.Frequency,
                [NumericData.Volume] = BeatmaniaPcConstants.VolumeTable[definition.Volume],
                [NumericData.SourceVolume] = definition.Volume,
                [NumericData.Panning] = (float)(definition.Panning - 1) / 0x7E,
                [NumericData.SourcePanning] = definition.Panning,
                Samples = channels.Select(c => new Sample
                {
                    Data = c
                }).Cast<ISample>().ToList()
            };
        }
    }
}