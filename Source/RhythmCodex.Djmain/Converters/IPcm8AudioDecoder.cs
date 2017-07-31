using System.Collections.Generic;

namespace RhythmCodex.Djmain.Converters
{
    public interface IPcm8AudioDecoder
    {
        IList<float> Decode(IEnumerable<byte> data);
    }
}