using System.Collections.Generic;
using RhythmCodex.ImaAdpcm.Models;
using RhythmCodex.Infrastructure.Models;

namespace RhythmCodex.ImaAdpcm.Converters
{
    public interface IImaAdpcmDecoder
    {
        IList<ISound> Decode(ImaAdpcmChunk chunk);
    }
}