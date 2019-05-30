using System.Collections.Generic;
using RhythmCodex.Sounds.Models;
using RhythmCodex.Xa.Models;

namespace RhythmCodex.Xa.Converters
{
    public interface IXaDecoder
    {
        IList<ISound> Decode(XaChunk chunk);
    }
}