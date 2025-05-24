using System;
using System.Linq;
using RhythmCodex.IoC;
using RhythmCodex.Meta.Models;
using RhythmCodex.Sounds.Models;
using RhythmCodex.Twinkle.Model;

namespace RhythmCodex.Twinkle.Converters;

[Service]
public class TwinkleBeatmaniaSoundDecoder : ITwinkleBeatmaniaSoundDecoder
{
    public Sound Decode(TwinkleBeatmaniaSoundDefinition definition, ReadOnlySpan<byte> data)
    {
        var offset = definition.SampleStart << 1;
        var stereo = (definition.Flags0F & 0x80) != 0;
        var length = definition.SampleEnd - definition.SampleStart;
        var channels = Enumerable
            .Range(0, stereo ? 2 : 1)
            .Select(_ => new float[length])
            .ToArray();
        var channelCount = channels.Length;
        var total = length / (stereo ? 2 : 1);

        for (var i = 0; i < total; i++)
        {
            for (var c = 0; c < channelCount; c++, offset += 2)
            {
                var sample = (data[offset] << 8) | data[offset + 1];
                if (sample >= 0x8000)
                    sample = -(sample & 0x7FFF);

                channels[c][i] = (float)sample / 0x7FFF;
            }
        }

        var panning = (float) (definition.Panning - 1) / 0x7E;
        if (panning < 0)
            panning = 0;
        else if (panning > 1)
            panning = 1;
            
        return new Sound
        {
            [NumericData.Rate] = definition.Frequency,
            [NumericData.SourceRate] = definition.Frequency,
            [NumericData.Volume] = TwinkleConstants.VolumeTable[definition.Volume],
            [NumericData.SourceVolume] = definition.Volume,
            [NumericData.Panning] = panning,
            [NumericData.SourcePanning] = definition.Panning,
            [NumericData.Channel] = definition.Channel,
            Samples = channels.Select(c => new Sample
            {
                Data = c
            }).ToList()
        };
    }
}