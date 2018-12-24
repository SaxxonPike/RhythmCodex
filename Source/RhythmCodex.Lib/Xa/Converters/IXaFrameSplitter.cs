using System;
using System.Collections.Generic;

namespace RhythmCodex.Xa.Converters
{
    public interface IXaFrameSplitter
    {
        int GetStatus(ReadOnlyMemory<byte> frame, int channel);
        IEnumerable<int> Get4BitData(ReadOnlyMemory<byte> frame, int channel);
        IEnumerable<int> Get8BitData(ReadOnlyMemory<byte> frame, int channel);
    }
}