using System.Collections.Generic;
using RhythmCodex.Djmain.Model;
using RhythmCodex.Sounds.Models;

namespace RhythmCodex.Djmain.Converters;

public interface IDjmainSoundDecoder
{
    Dictionary<int, Sound> Decode(IEnumerable<KeyValuePair<int, DjmainSample>> samples);
}