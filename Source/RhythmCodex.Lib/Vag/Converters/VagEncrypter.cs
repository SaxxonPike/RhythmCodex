using System;
using RhythmCodex.IoC;
using RhythmCodex.Vag.Models;

namespace RhythmCodex.Vag.Converters
{
    [Service]
    public class VagEncrypter : IVagEncrypter
    {
        private readonly IVagDecrypter _vagDecrypter;

        public VagEncrypter(IVagDecrypter vagDecrypter)
        {
            _vagDecrypter = vagDecrypter;
        }
        
        public void Encrypt(ReadOnlySpan<float> input, Span<byte> output, int length, VagState state)
        {
            var filterCount = VagCoefficients.Coeff0.Length;
            var inOffset = 0;
            var outOffset = 0;
            var maxOffset = length - 27;
            var testDecodeBuffer = new float[28];

            while (inOffset < maxOffset)
            {
                var inBuffer = input.Slice(inOffset);
                var outBuffer = output.Slice(outOffset);

                // Deduce the magnitude.
                var min = float.MaxValue;
                var max = float.MinValue;
                for (var i = 0; i < 28; i++)
                {
                    if (inBuffer[i] < min)
                        min = inBuffer[i];
                    if (inBuffer[i] > max)
                        max = inBuffer[i];
                }

                var range = max - min;
                var multiplier = 8f;
                var magnitude = 0;
                while (range <= 1 && magnitude < 12)
                {
                    multiplier *= 2;
                    range *= 2;
                    magnitude++;
                }

                // Populate the frame.
                for (var i = 0; i < 28; i += 2)
                {
                    var sample0 = (int) Math.Round(inBuffer[i] * multiplier);
                    if (sample0 < -8)
                        sample0 = -8;
                    else if (sample0 > 7)
                        sample0 = 7;
                    var sample1 = (int) Math.Round(inBuffer[i + 1] * multiplier);
                    if (sample1 < -8)
                        sample1 = -8;
                    else if (sample1 > 7)
                        sample1 = 7;
                    outBuffer[2 + (i >> 1)] = unchecked((byte) ((sample0 & 0xF) | ((sample1 & 0xF) << 4)));
                }

                // Deduce the filter.
                byte bestFilter = 0;
                var bestFilterDiff = double.MaxValue;
                var prev0 = state.Prev0;
                var prev1 = state.Prev1;
                var bestPrev0 = prev0;
                var bestPrev1 = prev1;
                for (var i = 0; i < filterCount; i++)
                {
                    outBuffer[0] = unchecked((byte) (magnitude | (i << 4)));
                    _vagDecrypter.Decrypt(outBuffer, testDecodeBuffer, 16, state);
                    var diff = 0d;
                    for (var j = 0; j < 28; j++)
                        diff += Math.Pow((testDecodeBuffer[i] - inBuffer[i]) * 65536, 2);
                    if (diff < bestFilterDiff)
                    {
                        bestFilterDiff = diff;
                        bestFilter = outBuffer[0];
                        bestPrev0 = state.Prev0;
                        bestPrev1 = state.Prev1;
                    }
                    if (bestFilterDiff == 0)
                        break;
                    state.Prev0 = prev0;
                    state.Prev1 = prev1;
                }

                state.Prev0 = bestPrev0;
                state.Prev1 = bestPrev1;
                outBuffer[0] = bestFilter;
                inOffset += 28;
                outOffset += 16;
            }
        }
    }
}