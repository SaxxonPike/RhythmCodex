using System.IO;
using System.Linq;
using MP3Sharp;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;
using RhythmCodex.Metadatas.Models;
using RhythmCodex.Sounds.Converters;
using RhythmCodex.Sounds.Models;
using RhythmCodex.Sounds.Mp3.Converters;

namespace RhythmCodex.Plugin.MP3Sharp;

[Service]
public class Mp3Decoder(IAudioDsp audioDsp) : IMp3Decoder
{
    public Sound? Decode(Stream stream)
    {
        var inputStream = new MP3Stream(stream);
        var channels = inputStream.ChannelCount;
        var rate = inputStream.Frequency;
        var data = inputStream.ReadAllBytes();

        var result = audioDsp.BytesToSamples(
            data.Span,
            16,
            channels,
            false
        );

        foreach (var sample in result)
            sample[NumericData.Rate] = rate;

        return new Sound
        {
            Samples = result.ToList(),
            [NumericData.Rate] = rate
        };
    }
}