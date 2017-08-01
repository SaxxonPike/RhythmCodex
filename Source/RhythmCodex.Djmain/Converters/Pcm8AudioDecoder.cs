using System.Collections.Generic;
using System.Linq;

namespace RhythmCodex.Djmain.Converters
{
    public class Pcm8AudioDecoder : IPcm8AudioDecoder
    {
        public IList<float> Decode(IEnumerable<byte> data)
        {
            return data.Select(b => ((b ^ 0x80) - 0x80) / 128f).ToArray();
        }
    }
}
