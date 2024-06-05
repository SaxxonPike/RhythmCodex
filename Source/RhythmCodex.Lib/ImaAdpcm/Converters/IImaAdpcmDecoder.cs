using RhythmCodex.ImaAdpcm.Models;
using RhythmCodex.Sounds.Models;

namespace RhythmCodex.ImaAdpcm.Converters;

public interface IImaAdpcmDecoder
{
    Sound? Decode(ImaAdpcmChunk chunk);
}