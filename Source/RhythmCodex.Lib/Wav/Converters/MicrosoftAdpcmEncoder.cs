using System;
using System.IO;
using System.Linq;
using RhythmCodex.IoC;
using RhythmCodex.Sounds.Models;
using RhythmCodex.Wav.Models;

namespace RhythmCodex.Wav.Converters;

[Service]
public class MicrosoftAdpcmEncoder : IMicrosoftAdpcmEncoder
{
    public Memory<byte> Encode(Sound? sound, int samplesPerBlock)
    {
        var channelCount = sound.Samples.Count;
        var buffer = new float[samplesPerBlock];
        var max = sound.Samples.Select(s => s.Data.Length).Max();
        var remaining = max + 1;
        var output = new byte[GetBlockSize(samplesPerBlock, channelCount)];
        var offset = 0;
        var channels = sound.Samples.Select(s => s.Data).ToArray();
        var deltas = new int[channelCount];

        using var encoded = new MemoryStream();
        while (remaining > 0)
        {
            for (var channel = 0; channel < channelCount; channel++)
            {
                ReadOnlySpan<float> bufferSpan;
                var source = channels[channel].Span;
                var length = Math.Min(samplesPerBlock, source.Length - offset);
                if (length < samplesPerBlock)
                {
                    buffer.AsSpan(0, length).Fill(0);
                    source.Slice(offset, length).CopyTo(buffer);
                    bufferSpan = buffer;
                }
                else
                {
                    bufferSpan = source.Slice(offset, samplesPerBlock);
                }

                deltas[channel] = EncodeFrame(output, bufferSpan, channel, channelCount,
                    MicrosoftAdpcmConstants.DefaultCoefficients, deltas[channel]);
            }

            encoded.Write(output, 0, output.Length);
            offset += samplesPerBlock;
            remaining -= samplesPerBlock;
            output.AsSpan().Fill(0x00);
        }

        return encoded.ToArray();
    }

    public int GetBlockSize(int samplesPerBlock, int channels) => 
        ((samplesPerBlock - 2) * channels / 2) + (channels * 7);

    // returns delta
    private int EncodeFrame(Span<byte> frame, ReadOnlySpan<float> buffer, int channel, int channelCount,
        ReadOnlySpan<int> coefficients, int initialDelta)
    {
        // write starting sample to buffer
        var sample2 = ToSample(buffer[0]);
        var sample1 = ToSample(buffer[1]);
        var sample1Offset = channelCount * 3 + (channel << 1);
        var sample2Offset = channelCount * 5 + (channel << 1);
        frame[sample1Offset] = unchecked((byte) sample1);
        frame[sample1Offset + 1] = unchecked((byte) (sample1 >> 8));
        frame[sample2Offset] = unchecked((byte) sample2);
        frame[sample2Offset + 1] = unchecked((byte) (sample2 >> 8));

        // write starting delta to buffer
        var delta = SaturateDelta(initialDelta);
        var deltaOffset = channelCount + (channel << 1);
        frame[deltaOffset] = unchecked((byte) delta);
        frame[deltaOffset + 1] = unchecked((byte) (delta >> 8));

        // try each coefficient
        var coeffCount = coefficients.Length >> 1;
        var bestCoeffError = double.MaxValue;
        var bestCoeff = -1;
        int coeff1;
        int coeff2;

        for (var coeff = 0; coeff < coeffCount; coeff++)
        {
            coeff1 = coefficients[coeff << 1];
            coeff2 = coefficients[(coeff << 1) + 1];
            delta = SaturateDelta(initialDelta);
            sample2 = ToSample(buffer[0]);
            sample1 = ToSample(buffer[1]);
            var coeffError = 0d;
            for (var i = 0; i < buffer.Length; i++)
            {
                var (error, _, d, s1) = FindBestNybble(buffer[i], sample1, coeff1, sample2, coeff2, delta);
                coeffError += error;
                coeffError /= 2;
                sample2 = sample1;
                sample1 = s1;
                delta = d;

                if (coeffError > bestCoeffError)
                    break;
            }

            if (coeffError < bestCoeffError)
            {
                bestCoeffError = coeffError;
                bestCoeff = coeff;
            }

            if (bestCoeffError == 0)
                break;
        }

        // Populate the frame.
        var frameIndex = channelCount * 7;
        var channelIndex = -1;
        var bufferIndex = 2;
        delta = SaturateDelta(initialDelta);
        coeff1 = coefficients[bestCoeff << 1];
        coeff2 = coefficients[(bestCoeff << 1) + 1];
        frame[channel] = unchecked((byte) bestCoeff);

        while (frameIndex < frame.Length)
        {
            channelIndex++;
            if (channelIndex == channelCount)
                channelIndex = 0;

            if (channelIndex == channel)
            {
                var (_, data, d, s1) =
                    FindBestNybble(buffer[bufferIndex++], sample1, coeff1, sample2, coeff2, delta);
                frame[frameIndex] |= unchecked((byte) (data << 4));
                sample2 = sample1;
                sample1 = s1;
                delta = d;
            }

            channelIndex++;
            if (channelIndex == channelCount)
                channelIndex = 0;

            if (channelIndex == channel)
            {
                var (_, data, d, s1) =
                    FindBestNybble(buffer[bufferIndex++], sample1, coeff1, sample2, coeff2, delta);
                frame[frameIndex] |= unchecked((byte) data);
                sample2 = sample1;
                sample1 = s1;
                delta = d;
            }

            frameIndex++;
        }

        return delta;
    }
        
    private static (double error, int data, int d, int s1) FindBestNybble(float target, int s1, int ce1, int s2, int ce2,
        int d)
    {
        var bestNybble = -1;
        var bestError = double.MaxValue;
        var bestS1 = 0;
        var bestD = 0;
        var targetSample = ToSample(target);

        for (var data = 0; data < 16; data++)
        {
            var predictor = (s1 * ce1 + s2 * ce2) >> 8;
            predictor += ((data << 28) >> 28) * d;

            var newS1 = predictor;
            if (newS1 < -32768)
                newS1 = -32768;
            if (newS1 > 32767)
                newS1 = 32767;

            var newD = SaturateDelta((MicrosoftAdpcmConstants.AdaptationTable[data] * d) >> 8);

            double error = newS1 - targetSample;
            error *= error;
            if (error < bestError)
            {
                bestError = error;
                bestNybble = data;
                bestD = newD;
                bestS1 = newS1;
            }

            if (bestError == 0)
                break;
        }

        return (bestError, bestNybble, bestD, bestS1);
    }

    private static int SaturateDelta(int d) => 
        d < 16 ? 16 : d;

    private static int ToSample(float sample)
    {
        var s = (int) (sample * 32768);
        if (s > 32767)
            return 32767;
        if (s < -32768)
            return -32768;
        return s;
    }
}