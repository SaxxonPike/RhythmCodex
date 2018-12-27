using System;

namespace CSCore.Codecs.FLAC
{
    internal sealed partial class FlacSubFrameLPC
    {
        private void RestoreLPCSignal32(Span<int> residual, Span<int> destination, int length, int order, int[] qlpCoeff,
            int lpcShiftNeeded)
        {
            var d = destination;
            var q = qlpCoeff;
            var ord1 = order - 1;

            var idx = order;
            for (var i = 0; i < length; i++)
            {
                var z = 0;
                var m = i;
                for (var k = ord1; k >= 0; k--)
                    z += q[k] * d[m++];
                d[idx] = residual[idx] + (z >> lpcShiftNeeded);
                idx++;
            }
        }

        private void RestoreLPCSignal64(Span<int> residual, Span<int> destination, int length, int order, int[] qlpCoeff,
            int lpcShiftNeeded)
        {
            var d = destination;
            var q = qlpCoeff;
            var ord1 = order - 1;

            var idx = order;
            for (var i = 0; i < length; i++)
            {
                var z = 0L;
                var m = i;
                for (var k = ord1; k >= 0; k--)
                    z += q[k] * d[m++];
                d[idx] = (int)(residual[idx] + (z >> lpcShiftNeeded));
                idx++;
            }
        }
    }
}

