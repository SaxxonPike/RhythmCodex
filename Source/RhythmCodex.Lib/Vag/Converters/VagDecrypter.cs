using System;
using RhythmCodex.IoC;
using RhythmCodex.Vag.Models;

namespace RhythmCodex.Vag.Converters
{
    [Service]
    public class VagDecrypter : IVagDecrypter
    {
        private static readonly int[] Coeff0 =
        {
            0, 60, 115, 98, 122
        };

        private static readonly int[] Coeff1 =
        {
            0, 0, -52, -55, -60
        };

        public void Decrypt(ReadOnlySpan<byte> input, Span<float> output, int length, VagDecodeState decodeState)
        {
            var inOffset = 0;
            var outOffset = 0;
            var last0 = decodeState.Prev0;
            var last1 = decodeState.Prev1;
            var enabled = decodeState.Enabled;

            while (inOffset < length)
            {
                var flags = input[inOffset + 1];

                if (!enabled)
                {
                    for (var i = 2; i < 16; i++)
                        output[outOffset++] = 0;
                }
                else
                {
                    var fm = input[inOffset];
                    var filter = fm >> 4;
                    var magnitude = fm & 0xF;

                    if (magnitude > 12 || filter > 4)
                    {
                        magnitude = 12;
                        filter = 0;
                    }

                    for (var i = 2; i < 16; i++)
                    {
                        var deltas = input[inOffset + i];
                        var delta0 = (deltas & 0x0F) << 28;
                        var delta1 = (deltas & 0xF0) << 24;
                        var coeff0 = Coeff0[filter];
                        var coeff1 = Coeff1[filter];

                        var filter0 = coeff0 * last0;
                        var filter1 = coeff1 * last1;
                        var sample = (delta0 >> (magnitude + 16)) + ((filter0 + filter1) >> 6);
                        if (sample > short.MaxValue)
                            sample = short.MaxValue;
                        else if (sample < short.MinValue)
                            sample = short.MinValue;
                        last1 = last0;
                        last0 = sample;
                        output[outOffset++] = sample / 32768f;

                        filter0 = coeff0 * last0;
                        filter1 = coeff1 * last1;
                        sample = (delta1 >> (magnitude + 16)) + ((filter0 + filter1) >> 6);
                        if (sample > short.MaxValue)
                            sample = short.MaxValue;
                        else if (sample < short.MinValue)
                            sample = short.MinValue;
                        last1 = last0;
                        last0 = sample;
                        output[outOffset++] = sample / 32768f;
                    }

                    if ((flags & 1) != 0)
                        enabled = false;
                }

                if ((flags & 5) == 4)
                    enabled = true;

                inOffset += 16;
            }

            decodeState.Prev0 = last0;
            decodeState.Prev1 = last1;
            decodeState.Enabled = enabled;
        }
    }
}