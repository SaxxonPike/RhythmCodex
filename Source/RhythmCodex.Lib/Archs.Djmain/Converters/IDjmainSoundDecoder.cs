using System.Collections.Generic;
using RhythmCodex.Archs.Djmain.Model;
using RhythmCodex.Sounds.Models;

namespace RhythmCodex.Archs.Djmain.Converters;

public interface IDjmainSoundDecoder
{
    Dictionary<int, Sound> Decode(IEnumerable<KeyValuePair<int, DjmainSample>> samples, bool swapStereo);
}