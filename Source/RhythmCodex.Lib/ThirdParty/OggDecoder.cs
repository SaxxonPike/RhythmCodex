using System.Collections.Generic;
using System.IO;
using System.Linq;
using NVorbis;
using RhythmCodex.Attributes;
using RhythmCodex.Infrastructure;
using RhythmCodex.Infrastructure.Models;

namespace RhythmCodex.ThirdParty
{
    [Service]
    public class OggDecoder : IOggDecoder
    {
        public ISound Decode(Stream stream)
        {
            using (var reader = new VorbisReader(stream, false))
            {
                reader.ClipSamples = false;
                
                var channels = reader.Channels;
                var rate = reader.SampleRate;
                var rawData = new List<float>();
                var rawDataBuffer = new float[4096];

                while (true)
                {
                    var samplesRead = reader.ReadSamples(rawDataBuffer, 0, rawDataBuffer.Length);
                    if (samplesRead == 0)
                        break;
                    rawData.AddRange(rawDataBuffer.Take(samplesRead));
                }
                
                var result = new Sound
                {
                    Samples = rawData.Deinterleave(1, channels)
                        .Select(samples => new Sample
                        {
                            Data = samples.ToArray(),
                            [NumericData.Rate] = rate
                        })
                        .Cast<ISample>()
                        .ToList()
                };

                return result;                
            }
        }
    }
}