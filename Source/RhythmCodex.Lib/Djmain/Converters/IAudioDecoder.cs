using System.Collections.Generic;

namespace RhythmCodex.Djmain.Converters
{
    public interface IAudioDecoder
    {
        IList<float> DecodeDpcm(IEnumerable<byte> data);
        IList<float> DecodePcm8(IEnumerable<byte> data);
        IList<float> DecodePcm16(IEnumerable<byte> data);
    }
}