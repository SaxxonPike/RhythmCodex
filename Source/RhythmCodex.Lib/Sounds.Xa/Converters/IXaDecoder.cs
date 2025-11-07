using System.Collections.Generic;
using RhythmCodex.Sounds.Models;
using RhythmCodex.Sounds.Xa.Models;

namespace RhythmCodex.Sounds.Xa.Converters;

public interface IXaDecoder
{
    List<Sound?> Decode(XaChunk chunk);
}