using System.Collections.Generic;

namespace RhythmCodex.Infrastructure.Converters
{
    public interface IBcd
    {
        int FromBcd(IEnumerable<byte> bytes);
        int FromBcd(byte b);
    }
}