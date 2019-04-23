using System;
using System.Collections.Generic;

namespace RhythmCodex.Xa.Converters
{
    public interface IXaFrameSplitter
    {
        int GetStatus(ReadOnlySpan<byte> frame, int channel);
        void Get4BitData(ReadOnlySpan<byte> frame, Span<int> buffer, int channel);
        void Get8BitData(ReadOnlySpan<byte> frame, Span<int> buffer, int channel);
    }
}