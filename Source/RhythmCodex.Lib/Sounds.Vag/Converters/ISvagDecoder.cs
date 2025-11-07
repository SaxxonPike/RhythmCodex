using RhythmCodex.Sounds.Models;
using RhythmCodex.Sounds.Vag.Models;

namespace RhythmCodex.Sounds.Vag.Converters;

public interface ISvagDecoder
{
    Sound? Decode(SvagContainer container);
}