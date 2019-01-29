using System.IO;
using System.Linq;
using MP3Sharp;
using RhythmCodex.Attributes;
using RhythmCodex.Extensions;
using RhythmCodex.Infrastructure;
using RhythmCodex.Infrastructure.Models;
using RhythmCodex.IoC;

namespace RhythmCodex.ThirdParty
{
    [Service]
    public class Mp3Decoder : IMp3Decoder
    {
        public ISound Decode(Stream stream)
        {
            var inputStream = new MP3Stream(stream);
            var channels = inputStream.ChannelCount;
            var rate = inputStream.Frequency;
            var data = inputStream.ReadAllBytes();

            var result = new Sound
            {
                Samples = data
                    .Deinterleave(2, channels)
                    .Select(bytes => new Sample
                    {
                        Data = bytes.AsArray().Fuse().Select(s => s / 32768f).AsArray(),
                        [NumericData.Rate] = rate,
                    })
                    .Cast<ISample>()
                    .ToList()
            };

            return result;
        }
    }
}