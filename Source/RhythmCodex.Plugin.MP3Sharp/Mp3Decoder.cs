using System.IO;
using System.Linq;
using MP3Sharp;
using RhythmCodex.Extensions;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;
using RhythmCodex.Metadatas.Models;
using RhythmCodex.Sounds.Models;
using RhythmCodex.Sounds.Mp3.Converters;
using RhythmCodex.Utils.Cursors;

namespace RhythmCodex.Plugin.MP3Sharp;

[Service]
public class Mp3Decoder : IMp3Decoder
{
    public Sound Decode(Stream stream)
    {
        var inputStream = new MP3Stream(stream);
        var channels = inputStream.ChannelCount;
        var rate = inputStream.Frequency;
        var data = inputStream.ReadAllBytes();

        var result = new Sound
        {
            Samples = data
                .Span
                .Deinterleave(2, channels)
                .Select(bytes => new Sample
                {
                    Data = bytes.ToU16L().Select(s => s / 32768f).ToArray(),
                    [NumericData.Rate] = rate
                })
                .Cast<Sample>()
                .ToList()
        };

        return result;
    }
}