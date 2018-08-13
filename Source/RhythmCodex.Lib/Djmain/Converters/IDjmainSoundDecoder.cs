using System.Collections.Generic;
using RhythmCodex.Audio;
using RhythmCodex.Djmain.Model;

namespace RhythmCodex.Djmain.Converters
{
    public interface IDjmainSoundDecoder
    {
        IDictionary<int, ISound> Decode(IEnumerable<KeyValuePair<int, IDjmainSample>> samples);
    }
}