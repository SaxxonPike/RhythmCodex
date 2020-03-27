using RhythmCodex.Sounds.Models;
using RhythmCodex.Vag.Models;

namespace RhythmCodex.Vag.Converters
{
    public interface ISvagDecoder
    {
        ISound Decode(SvagContainer container);
    }
}