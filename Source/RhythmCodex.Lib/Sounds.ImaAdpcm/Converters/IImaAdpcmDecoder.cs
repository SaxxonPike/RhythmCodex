using RhythmCodex.Sounds.ImaAdpcm.Models;
using RhythmCodex.Sounds.Models;

namespace RhythmCodex.Sounds.ImaAdpcm.Converters;

public interface IImaAdpcmDecoder
{
    Sound? Decode(ImaAdpcmChunk chunk);
}