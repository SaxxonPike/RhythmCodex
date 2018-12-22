using System;
using System.Collections.Generic;

namespace RhythmCodex.Infrastructure.Converters
{
    public interface IBitter
    {
        int ToInt32(byte lsb, byte b, byte c, byte msb);
        int ToInt32(IEnumerable<byte> bytes);
        int ToInt32(Span<byte> bytes);
    }
}