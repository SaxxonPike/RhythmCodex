using RhythmCodex.Sounds.Models;
using RhythmCodex.Vag.Models;

namespace RhythmCodex.Vag.Converters;

public interface IVagDecoder
{
    Sound? Decode(VagChunk? chunk);
}