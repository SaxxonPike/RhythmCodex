using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using RhythmCodex.IoC;
using RhythmCodex.Sounds.Models;
using RhythmCodex.Wav.Models;

namespace RhythmCodex.Wav.Converters;

[Service]
public class MicrosoftAdpcmDecoder : IMicrosoftAdpcmDecoder
{
    // Reference: https://wiki.multimedia.cx/index.php/Microsoft_ADPCM

    public Sound? Decode(
        ReadOnlySpan<byte> data,
        IWaveFormat fmtChunk,
        MicrosoftAdpcmFormat microsoftAdpcmFormat)
    {
        var channels = fmtChunk.Channels;
        var channelSamplesPerFrame = microsoftAdpcmFormat.SamplesPerBlock;
        var buffer = new float[channelSamplesPerFrame];
        var frameSize = fmtChunk.BlockAlign;
        var max = data.Length / frameSize * frameSize;

        var output = Enumerable
            .Range(0, channels)
            .Select(_ => new List<float>())
            .ToArray();

        // Apply coefficients.
        int[] coefficients;

        if (microsoftAdpcmFormat.Coefficients.Count < MicrosoftAdpcmConstants.CoefficientTableMinimumSize)
        {
            coefficients = new int[MicrosoftAdpcmConstants.CoefficientTableMinimumSize];
            MicrosoftAdpcmConstants.CreateCoefficientTable().CopyTo(coefficients);
            microsoftAdpcmFormat.Coefficients.CopyTo(coefficients);
        }
        else
        {
            coefficients = microsoftAdpcmFormat.Coefficients.ToArray();
        }

        // Apply adaptation table.
        var adaptationTable = MicrosoftAdpcmConstants.CreateAdaptationTable().ToArray();

        for (var offset = 0; offset < max; offset += frameSize)
        {
            var mem = data.Slice(offset, frameSize);
            for (var channel = 0; channel < channels; channel++)
            {
                DecodeFrame(mem, buffer, channel, channels, coefficients, adaptationTable);
                output[channel].AddRange(buffer);
            }
        }

        return new Sound
        {
            Samples = output
                .Select(s => new Sample { Data = s.ToArray() })
                .ToList()
        };
    }

    private static int DecodeFrame(
        ReadOnlySpan<byte> frame,
        Span<float> buffer,
        int channel,
        int channelCount,
        ReadOnlySpan<int> coefficients,
        ReadOnlySpan<int> adaptationTable)
    {
        // Read block header.
        var control = frame[channel];
        var coeff1 = coefficients[control << 1];
        var coeff2 = coefficients[(control << 1) + 1];
        var index = channelCount + channel * 2;
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
            channelIndex++;
            if (channelIndex == channelCount)
                channelIndex = 0;

            if (channelIndex == channel)
                buffer[bufferIndex++] = DecodeNybble(frame[index] >> 4, adaptationTable);

            channelIndex++;
            if (channelIndex == channelCount)
                channelIndex = 0;

            if (channelIndex == channel)
                buffer[bufferIndex++] = DecodeNybble(frame[index] & 0xF, adaptationTable);

            index++;
            continue;

            float DecodeNybble(int data, ReadOnlySpan<int> at)
            {
                var predictor = (sample1 * coeff1 + sample2 * coeff2) / 256;
                predictor += ((data & 0x08) != 0 ? data - 0x10 : data) * delta;

                sample2 = sample1;
                sample1 = predictor switch
                {
                    < -32768 => -32768,
                    > 32767 => 32767,
                    _ => predictor
                };

                delta = at[data] * delta / 256;
                if (delta < 16)
                    delta = 16;

                return sample1 / 32768f;
            }
        }

        return bufferIndex;
    }
}