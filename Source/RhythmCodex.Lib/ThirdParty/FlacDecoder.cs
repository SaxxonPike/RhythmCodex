using System.IO;
using System.Linq;
using CSCore.Codecs.FLAC;
using RhythmCodex.Attributes;
using RhythmCodex.Extensions;
using RhythmCodex.Infrastructure;
using RhythmCodex.Infrastructure.Models;
using RhythmCodex.IoC;

namespace RhythmCodex.ThirdParty
{
    [Service]
    public class FlacDecoder : IFlacDecoder
    {
        public ISound Decode(Stream stream)
        {
            using (var inputStream = new FlacFile(stream))
            {
                var samples = StreamExtensions.ReadAllBytes(inputStream.Read)
                    .Deinterleave(2, inputStream.WaveFormat.Channels)
                    .Select(bytes => new Sample
                    {
                        Data = bytes.AsArray().Fuse().Select(s => s / 32768f).AsArray()
                    })
                    .Cast<ISample>()
                    .ToList();
                
                return new Sound
                {
                    Samples = samples,
                    [NumericData.Rate] = inputStream.WaveFormat.SampleRate
                };
            }
        }
    }
}