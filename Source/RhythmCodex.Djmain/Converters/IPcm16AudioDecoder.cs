using System.Collections.Generic;

namespace RhythmCodex.Djmain.Converters
{
    public interface IPcm16AudioDecoder
    {
        IList<float> Decode(IEnumerable<byte> data);
    }
}