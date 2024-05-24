using RhythmCodex.Sounds.Models;
using RhythmCodex.Vag.Models;

namespace RhythmCodex.Vag.Converters;

public interface ISvagDecoder
{
    Sound? Decode(SvagContainer container);
}