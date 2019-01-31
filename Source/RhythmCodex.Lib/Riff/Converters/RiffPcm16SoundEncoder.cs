using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using RhythmCodex.Attributes;
using RhythmCodex.Infrastructure;
using RhythmCodex.Infrastructure.Models;
using RhythmCodex.IoC;
using RhythmCodex.Riff.Models;

namespace RhythmCodex.Riff.Converters
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
                    for (var j = 0; j < sound.Samples.Count; j++)
                    {
                        var sample = sound.Samples[j];
                        var source = sample.Data;
                        var sourceValue = i < source.Count ? source[i] : 0f;
                        var value = Math.Round(sourceValue * 32767f);
                        if (value > 32767f)
                            value = 32767f;
                        else if (value < -32767f)
                            value = -32767f;
                        writer.Write((short) value);
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