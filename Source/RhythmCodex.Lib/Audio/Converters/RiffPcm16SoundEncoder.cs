using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using RhythmCodex.Attributes;
using RhythmCodex.Audio.Models;
using RhythmCodex.Infrastructure;

namespace RhythmCodex.Audio.Converters
{
    [Service]
    public class RiffPcm16SoundEncoder : IRiffPcm16SoundEncoder
    {
        private readonly IRiffFormatEncoder _formatEncoder;

        public RiffPcm16SoundEncoder(IRiffFormatEncoder formatEncoder)
        {
            _formatEncoder = formatEncoder;
        }
        
        public IRiffContainer Encode(ISound sound)
        {
            var sampleRate = (int) (sound[NumericData.Rate] ?? 44100);
            var channels = sound.Samples.Count;
            var byteRate = sampleRate * channels * 2;
            var container = new RiffContainer
            {
                Format = "WAVE",
                Chunks = new List<IRiffChunk>()
            };
            
            var format = new RiffFormat
            {
                Format = 1,
                SampleRate = sampleRate,
                Channels = channels,
                ByteRate = byteRate,
                BitsPerSample = 16,
                BlockAlign = channels * 2,
                ExtraData = new byte[0]
            };

            container.Chunks.Add(_formatEncoder.Encode(format));

            var totalSamples = sound.Samples.Max(s => s.Data.Count);
            
            using (var stream = new MemoryStream())
            using (var writer = new BinaryWriter(stream))
            {
                for (var i = 0; i < totalSamples; i++)
                {
                    foreach (var sample in sound.Samples)
                    {
                        var source = sample.Data;
                        var value = source.Count < i ? source[i] * 32767 : 0;
                        if (value > 32767)
                            value = 32767;
                        else if (value < -32767)
                            value = -32767;
                        writer.Write((short)value);
                    }
                }
                
                container.Chunks.Add(new RiffChunk
                {
                    Id = "data",
                    Data = stream.ToArray()
                });
            }

            return container;
        }
    }
}