using System;
using RhythmCodex.IoC;
using RhythmCodex.Sounds.Vag.Models;

namespace RhythmCodex.Sounds.Vag.Converters;

[Service]
public class VagDecrypter : IVagDecrypter
{
    public int Decrypt(ReadOnlySpan<byte> input, Span<float> output, int length, VagState state)
    {
        var table0 = VagCoefficients.Coeff0;
        var table1 = VagCoefficients.Coeff1;
        var inOffset = 0;
        var outOffset = 0;
        var last0 = state.Prev0;
        var last1 = state.Prev1;
        var enabled = state.Enabled;
        var maxLength = Math.Min(length, input.Length - 0x0F);

        while (inOffset < length)
        {
            if (inOffset >= maxLength)
            {
                enabled = false;
            }

            var flags = input[inOffset + 1];

            if ((flags & 5) == 4)
                enabled = true;

            if ((flags & 7) == 6)
                state.LoopStart = outOffset;

            if (!enabled)
            {
                output.Slice(outOffset, 28).Clear();
                outOffset += 28;
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

                magnitude += 16;

                var coeff0 = table0[filter];
                var coeff1 = table1[filter];

                for (var i = 2; i < 16; i++)
                {
                    var deltas = input[inOffset + i];
                    var delta0 = (deltas & 0x0F) << 28;
                    var delta1 = (deltas & 0xF0) << 24;

                    var filter0 = coeff0 * last0;
                    var filter1 = coeff1 * last1;
                    var sample = Math.Clamp((delta0 >> magnitude) + ((filter0 + filter1) >> 6), 
                        short.MinValue, short.MaxValue);
                    last1 = last0;
                    last0 = sample;
                    output[outOffset++] = sample / 32768f;

                    filter0 = coeff0 * last0;
                    filter1 = coeff1 * last1;
                    sample = Math.Clamp((delta1 >> magnitude) + ((filter0 + filter1) >> 6),
                        short.MinValue, short.MaxValue);
                    last1 = last0;
                    last0 = sample;
                    output[outOffset++] = sample / 32768f;
                }

                if ((flags & 1) != 0)
                    enabled = false;
            }

            if ((flags & 1) != 0 && state.LoopStart != null)
                state.LoopEnd = outOffset;

            inOffset += 16;
        }

        state.Prev0 = last0;
        state.Prev1 = last1;
        state.Enabled = enabled;

        return outOffset;
    }
}