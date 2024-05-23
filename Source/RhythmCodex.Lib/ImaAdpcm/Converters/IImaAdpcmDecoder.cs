using System.Collections.Generic;
using RhythmCodex.ImaAdpcm.Models;
using RhythmCodex.Sounds.Models;

namespace RhythmCodex.ImaAdpcm.Converters;

public interface IImaAdpcmDecoder
{
    IList<Sound?> Decode(ImaAdpcmChunk chunk);
}