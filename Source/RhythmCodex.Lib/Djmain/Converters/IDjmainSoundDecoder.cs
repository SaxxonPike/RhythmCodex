using System.Collections.Generic;
using RhythmCodex.Djmain.Model;
using RhythmCodex.Sounds.Models;

namespace RhythmCodex.Djmain.Converters
{
    public interface IDjmainSoundDecoder
    {
        IDictionary<int, ISound> Decode(IEnumerable<KeyValuePair<int, IDjmainSample>> samples);
    }
}