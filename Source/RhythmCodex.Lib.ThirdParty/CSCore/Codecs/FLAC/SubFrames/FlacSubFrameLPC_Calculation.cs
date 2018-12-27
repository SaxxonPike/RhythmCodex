using System;

namespace CSCore.Codecs.FLAC
{
    internal sealed partial class FlacSubFrameLPC
    {
        private void RestoreLPCSignal32(ReadOnlySpan<int> residual, Span<int> destination, int length, int order,
            ReadOnlySpan<int> qlpCoeff,
            int lpcShiftNeeded)
        {
            var ord1 = order - 1;
            var idx = order;

            for (var i = 0; i < length; i++)
            {
                var z = 0;
                var m = i;
                for (var k = ord1; k >= 0; k--)
                    z += qlpCoeff[k] * destination[m++];
                destination[idx] = residual[idx] + (z >> lpcShiftNeeded);
                idx++;
            }
        }

        private void RestoreLPCSignal64(ReadOnlySpan<int> residual, Span<int> destination, int length, int order,
            ReadOnlySpan<int> qlpCoeff,
            int lpcShiftNeeded)
        {
            var ord1 = order - 1;
            var idx = order;

            for (var i = 0; i < length; i++)
            {
                var z = 0L;
                var m = i;
                for (var k = ord1; k >= 0; k--)
                    z += qlpCoeff[k] * destination[m++];
                destination[idx] = (int) (residual[idx] + (z >> lpcShiftNeeded));
                idx++;
            }
        }
    }
}