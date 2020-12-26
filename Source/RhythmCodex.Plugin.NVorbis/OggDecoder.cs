using System.Collections.Generic;
using System.IO;
using System.Linq;
using RhythmCodex.Extensions;
using RhythmCodex.IoC;
using RhythmCodex.Meta.Models;
using RhythmCodex.Plugin.NVorbis.Lib;
using RhythmCodex.Sounds.Models;
using RhythmCodex.ThirdParty;

namespace RhythmCodex.Plugin.NVorbis
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