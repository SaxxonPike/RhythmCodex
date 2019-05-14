using System;
using RhythmCodex.IoC;
using RhythmCodex.Vag.Models;

namespace RhythmCodex.Vag.Converters
{
    [Service]
    public class VagEncrypter : IVagEncrypter
    {
        public void Encrypt(ReadOnlySpan<float> input, Span<byte> output, int length, VagState state)
        {
            var statePrev0 = state.Prev0;
            var statePrev1 = state.Prev1;
            var filterCount = VagCoefficients.Coeff0.Length;
            const int maxMagnitude = 12;
            const int magnitudeCount = 13;
            var inOffset = 0;
            var outOffset = 0;
            var maxOffset = length - 27;
            var frameDiff = new double[filterCount * magnitudeCount];
            var frameDiffSpan = frameDiff.AsSpan();
            var workBuffer = new byte[16 * filterCount * magnitudeCount];
            var workBufferSpan = workBuffer.AsSpan();
            var last0Buffer = new int[filterCount * magnitudeCount];
            var last1Buffer = new int[filterCount * magnitudeCount];
            var filterMagnitude = new int[filterCount];
            var inputSampleBuffer = new int[28];
            var lastOutOffset = -16;

            while (inOffset < maxOffset)
            {
                var inBuffer = input.Slice(inOffset);
                var outBuffer = output.Slice(outOffset);

                workBufferSpan.Fill(0x00);
                frameDiffSpan.Fill(0f);

                for (var index = 0; index < 28; index++)
                    inputSampleBuffer[index] = (int) (inBuffer[index] * 32768f);

                for (var filter = 0; filter < filterCount; filter++)
                {
                    filterMagnitude[filter] = maxMagnitude;
                    var coeff0 = VagCoefficients.Coeff0[filter];
                    var coeff1 = VagCoefficients.Coeff1[filter];
                    for (var magnitude = 0; magnitude < filterMagnitude[filter]; magnitude++)
                    {
                        var diffIndex = filter + magnitude * filterCount;
                        var workBufferIndex = diffIndex * 16;
                        var last0 = statePrev0;
                        var last1 = statePrev1;
                        workBuffer[workBufferIndex] = unchecked((byte) (magnitude | (filter << 4)));
                        for (var index = 0; index < 28; index++)
                        {
                            var filter0 = last0 * coeff0;
                            var filter1 = last1 * coeff1;
                            var inputSample = inputSampleBuffer[index];
                            var sampleToEncode = inputSample - ((filter0 + filter1) >> 6);
                            if (sampleToEncode > 32767)
                                sampleToEncode = 32767;
                            if (sampleToEncode < -32768)
                                sampleToEncode = -32768;

                            var nybble = ((sampleToEncode << (magnitude + 16)) >> 28) & 0xF;
                            if (nybble == 0x8 || nybble == 0x7)
                                filterMagnitude[filter] = Math.Min(filterMagnitude[filter], magnitude);

                            var encodedSample = ((nybble << 28) >> (magnitude + 16)) + ((filter0 + filter1) >> 6);
                            double outDiff = encodedSample - inputSample;
                            frameDiff[diffIndex] += outDiff * outDiff;

                            var shift = (index & 1) << 2;
                            var workIndex = 2 + (index >> 1);
                            workBuffer[workBufferIndex + workIndex] |= unchecked((byte) (nybble << shift));

                            last1 = last0;
                            last0 = encodedSample;
                        }

                        last0Buffer[diffIndex] = last0;
                        last1Buffer[diffIndex] = last1;
                    }
                }

                // Determine the most accurate frame
                var bestFrameDiffIndex = 0;
                var bestFrameDiff = double.MaxValue;
                for (var filter = 0; filter < filterCount; filter++)
                {
                    var highestMagnitude = filterMagnitude[filter];
                    var lowestMagnitude = Math.Max(0, highestMagnitude - 1);
                    for (var magnitude = highestMagnitude; magnitude >= lowestMagnitude; magnitude--)
                    {
                        var diffIndex = filter + magnitude * filterCount;
                        if (frameDiff[diffIndex] >= bestFrameDiff)
                            continue;

                        bestFrameDiffIndex = diffIndex;
                        bestFrameDiff = frameDiff[diffIndex];
                    }
                }

                // Write out the best frame.
                workBufferSpan.Slice(bestFrameDiffIndex * 16, 16).CopyTo(outBuffer);
                
                // If the frame is empty, write out 0x0C for filter/magnitude.
                var isEmptyFrame = true;
                for (var i = 2; i < 16; i++)
                {
                    if (outBuffer[i] != 0)
                    {
                        isEmptyFrame = false;
                        break;
                    }
                }

                if (isEmptyFrame)
                    outBuffer[0] = 0x0C;
                
                statePrev1 = last1Buffer[bestFrameDiffIndex];
                statePrev0 = last0Buffer[bestFrameDiffIndex];
                lastOutOffset = outOffset;
                inOffset += 28;
                outOffset += 16;
            }

            // Mark the end of the stream.
            if (lastOutOffset >= 0)
                output[lastOutOffset + 1] |= 0x01;

            // Mark the start of the stream.
            if (outOffset >= 0)
                output[1] |= 0x04;

            state.Prev0 = statePrev0;
            state.Prev1 = statePrev1;
        }
    }
}