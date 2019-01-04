using System;
using System.Collections.Generic;
using System.Linq;
using RhythmCodex.Infrastructure;
using RhythmCodex.Infrastructure.Models;
using RhythmCodex.Wav.Models;

namespace RhythmCodex.Wav.Converters
{
    [Service]
    public class MicrosoftAdpcmDecoder : IMicrosoftAdpcmDecoder
    {
        // Reference: https://wiki.multimedia.cx/index.php/Microsoft_ADPCM

        private static readonly int[] AdaptationTable =
        {
            230, 230, 230, 230, 307, 409, 512, 614,
            768, 614, 512, 409, 307, 230, 230, 230
        };

        private static readonly int[] DefaultCoefficients =
        {
            256, 0,
            512, -256,
            0, 0,
            192, 64,
            240, 0,
            460, -208,
            392, -232
        };

        private static readonly int[] IndexTable =
        {
            0, 1, 2, 3, 4, 5, 6, 7, -8, -7, -6, -5, -4, -3, -2, -1
        };

        public ISound Decode(ReadOnlySpan<byte> data, WaveFmtChunk fmtChunk, MsAdpcmFormat msAdpcmFormat)
        {
            var channels = fmtChunk.Channels;
            var channelSamplesPerFrame = msAdpcmFormat.SamplesPerBlock;
            var buffer = new float[channelSamplesPerFrame];
            var frameSize = fmtChunk.BlockAlign;
            var max = data.Length / frameSize * frameSize;
            var output = Enumerable.Range(0, channels).Select(i => new List<float>()).ToArray();

            // Apply coefficients
            var coefficients =
                new int[Math.Max(DefaultCoefficients.Length, msAdpcmFormat.Coefficients.Length)].AsMemory();
            DefaultCoefficients.AsSpan().CopyTo(coefficients.Span);
            msAdpcmFormat.Coefficients.AsSpan().CopyTo(coefficients.Span);

            for (var offset = 0; offset < max; offset += frameSize)
            {
                var mem = data.Slice(offset, frameSize);
                for (var channel = 0; channel < channels; channel++)
                {
                    DecodeFrame(mem, buffer, channel, channels, coefficients.Span);
                    output[channel].AddRange(buffer);
                }
            }

            return new Sound
            {
                Samples = output.Select(s => new Sample {Data = s}).Cast<ISample>().ToList()
            };
        }

        private int DecodeFrame(ReadOnlySpan<byte> frame, Span<float> buffer, int channel, int channelCount,
            ReadOnlySpan<int> coefficients)
        {
            // Read block header.
            var control = frame[channel];
            var coeff1 = coefficients[control << 1];
            var coeff2 = coefficients[(control << 1) + 1];
            var index = channelCount;
            var delta = ((frame[index] | (frame[index + 1] << 8)) << 16) >> 16;
            index += channelCount * 2;
            var sample1 = ((frame[index] | (frame[index + 1] << 8)) << 16) >> 16;
            index += channelCount * 2;
            var sample2 = ((frame[index] | (frame[index + 1] << 8)) << 16) >> 16;

            // Configure block.
            index = 7 * channelCount;
            var max = frame.Length;
            var bufferIndex = 0;
            var channelIndex = -1;

            // Output the first two discrete samples.
            buffer[bufferIndex++] = sample2 / 32767f;
            buffer[bufferIndex++] = sample1 / 32767f;

            // Proces the rest of the samples.
            while (index < max)
            {
                float DecodeNybble(int data)
                {
                    var predictor = (sample1 * coeff1 + sample2 * coeff2) / 256;
                    predictor += ((data & 0x08) != 0 ? (data - 0x10) : data) * delta;

                    sample2 = sample1;
                    sample1 = predictor;
                    
                    if (sample1 < -32768)
                        sample1 = -32768;
                    if (sample1 > 32767)
                        sample1 = 32767;

                    delta = AdaptationTable[data] * delta / 256;
                    if (delta < 16)
                        delta = 16;

                    return sample1 / 32768f;
                }

                channelIndex++;
                if (channelIndex == channelCount)
                    channelIndex = 0;

                if (channelIndex == channel)
                    buffer[bufferIndex++] = DecodeNybble(frame[index] >> 4);

                channelIndex++;
                if (channelIndex == channelCount)
                    channelIndex = 0;

                if (channelIndex == channel)
                    buffer[bufferIndex++] = DecodeNybble(frame[index] & 0xF);

                index++;
            }

            return bufferIndex;
        }
    }
}