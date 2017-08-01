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
                while (true)
                {
                    if (!e.MoveNext())
                        yield break;

                    var low = e.Current;
                    var high = e.MoveNext() ? e.Current : 0;
                    
                    yield return (((low | (high << 8)) << 16) >> 16) / 32768f;
                }
            }
        }
    }
}
