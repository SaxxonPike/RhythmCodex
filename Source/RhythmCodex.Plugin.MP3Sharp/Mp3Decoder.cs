using System.IO;
using System.Linq;
using RhythmCodex.Extensions;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;
using RhythmCodex.Meta.Models;
using RhythmCodex.Plugin.MP3Sharp.Lib;
using RhythmCodex.Sounds.Models;
using RhythmCodex.ThirdParty;

namespace RhythmCodex.Plugin.MP3Sharp
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