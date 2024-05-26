using System;
using System.IO;
using System.Linq;
using MP3Sharp;
using RhythmCodex.Extensions;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;
using RhythmCodex.Meta.Models;
using RhythmCodex.Mp3.Converters;
using RhythmCodex.Sounds.Models;

namespace RhythmCodex.Plugin.MP3Sharp;

[Service]
public class Mp3Decoder : IMp3Decoder
{
    public Sound? Decode(Stream stream)
    {
        var inputStream = new MP3Stream(stream);
        var channels = inputStream.ChannelCount;
        var rate = inputStream.Frequency;
        var data = inputStream.ReadAllBytes();

        var result = new Sound
        {
            Samples = data
                .AsSpan()
                .Deinterleave(2, channels)
                .Select(bytes => new Sample
                {
                    Data = bytes.Fuse().Select(s => s / 32768f).ToArray(),
                    [NumericData.Rate] = rate
                })
                .Cast<Sample>()
                .ToList()
        };

        return result;
    }
}