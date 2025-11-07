using RhythmCodex.Sounds.Models;
using RhythmCodex.Sounds.Vag.Models;

namespace RhythmCodex.Sounds.Vag.Converters;

public interface IVagDecoder
{
    Sound Decode(VagChunk chunk);
}