using System.Collections.Generic;
using System.Linq;

namespace RhythmCodex.Djmain.Converters
{
    public class Pcm16AudioDecoder : IPcm16AudioDecoder
    {
        public IList<float> Decode(IEnumerable<byte> data)
        {
            return DecodeData(data).ToArray();
        }

        private static IEnumerable<float> DecodeData(IEnumerable<byte> data)
        {
            using (var e = data.GetEnumerator())
            {
                if (!e.MoveNext())
                    yield break;

                var low = e.Current;
                
                if (!e.MoveNext())
                    yield break;

                var high = e.Current;

                yield return low | (high << 8);
            }
        }
    }
}
