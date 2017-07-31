using System.Collections.Generic;

namespace RhythmCodex.Djmain.Converters
{
    public interface IDpcmAudioDecoder
    {
        IList<float> Decode(IEnumerable<byte> data);
    }
}