using RhythmCodex.Infrastructure.Models;
using RhythmCodex.Xa.Models;

namespace RhythmCodex.Xa.Converters
{
    public interface IXaDecoder
    {
        ISound Decode(XaChunk chunk);
    }
}