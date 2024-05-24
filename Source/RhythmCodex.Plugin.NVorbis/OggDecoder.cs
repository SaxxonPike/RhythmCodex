using System;
using System.IO;
using System.Linq;
using NVorbis;
using RhythmCodex.Extensions;
using RhythmCodex.IoC;
using RhythmCodex.Meta.Models;
using RhythmCodex.Ogg.Converters;
using RhythmCodex.Sounds.Models;

namespace RhythmCodex.Plugin.NVorbis;

[Service]
public class OggDecoder : IOggDecoder
{
    public Sound Decode(Stream stream)
    {
        using var reader = new VorbisReader(stream, false);
        reader.ClipSamples = false;
        var channels = reader.Channels;
        var rate = reader.SampleRate;

        var rawData = new float[reader.TotalSamples];
        reader.ReadSamples(rawData);
        
        var result = new Sound
        {
            Samples = rawData
                .AsSpan()
                .Deinterleave(1, channels)
                .Select(samples => new Sample
                {
                    Data = samples.ToArray(),
                    [NumericData.Rate] = rate
                })
                .ToList()
        };

        return result;
    }
}