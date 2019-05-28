using System;
using System.IO;
using System.Linq;
using RhythmCodex.Extensions;
using RhythmCodex.IoC;
using RhythmCodex.Sounds.Models;
using RhythmCodex.Wav.Models;

namespace RhythmCodex.Wav.Converters
{
    [Service]
    public class MicrosoftAdpcmEncoder : IMicrosoftAdpcmEncoder
    {
        public byte[] Encode(ISound sound, IWaveFormat fmtChunk, MicrosoftAdpcmFormat microsoftAdpcmFormat)
        {
            var channelCount = fmtChunk.Channels;
            var samplesPerChannel = microsoftAdpcmFormat.SamplesPerBlock / channelCount;
            var buffer = new float[samplesPerChannel];
            var max = sound.Samples.Select(s => s.Data.Count).Max();
            var remaining = max + 1;
            var output = new byte[(samplesPerChannel * channelCount / 2) + (channelCount * 7)];
            var offset = 0;
            var channels = sound.Samples.Select(s => s.Data.AsArray()).ToArray();
            
            using (var encoded = new MemoryStream())
            {
                while (remaining > 0)
                {
                    for (var channel = 0; channel < channelCount; channel++)
                    {
                        ReadOnlySpan<float> bufferSpan;
                        var source = channels[channel];
                        var length = Math.Min(samplesPerChannel, source.Length - offset);
                        if (length < samplesPerChannel)
                        {
                            buffer.AsSpan().Slice(length).Fill(0);
                            source.AsSpan().Slice(offset, length).CopyTo(buffer);
                            bufferSpan = buffer;
                        }
                        else
                        {
                            bufferSpan = source.AsSpan().Slice(offset, samplesPerChannel);
                        }
                    
                        EncodeFrame(output, bufferSpan, channel, channelCount, MicrosoftAdpcmConstants.DefaultCoefficients);
                    }

                    remaining -= microsoftAdpcmFormat.SamplesPerBlock;
                }

                return encoded.ToArray();
            }
        }

        private void EncodeFrame(Span<byte> frame, ReadOnlySpan<float> buffer, int channel, int channelCount,
            ReadOnlySpan<int> coefficients)
        {
            var coeffCount = coefficients.Length >> 1;
            
            
            // frame[channel] = control
            // frame[(channelCount * 7) + channel] = delta
            // frame[(channelCount * 21) + channel] = sample1
            // frame[(channelCount * 35) + channel] = sample2
        }
    }
}