using System.Collections.Generic;
using System.Linq;

namespace RhythmCodex.Djmain.Converters
{
    public class DpcmAudioDecoder : IDpcmAudioDecoder
    {
        private static readonly int[] DpcmTable =
        {
            0x00, 0x01, 0x02, 0x04,
            0x08, 0x10, 0x20, 0x40,
            0x00, 0xC0, 0xE0, 0xF0,
            0xF8, 0xFC, 0xFE, 0xFF
        };

        public IList<float> Decode(IEnumerable<byte> data)
        {
            return DecodeData(data).ToArray();
        }

        private static IEnumerable<float> DecodeData(IEnumerable<byte> data)
        {
            var accumulator = 0;
            foreach (var b in data)
            {
                accumulator = (accumulator + DpcmTable[b & 0xF]) & 0xFF;
                yield return ((accumulator ^ 0x80) - 0x80) / 128f;
                accumulator = (accumulator + DpcmTable[b >> 4]) & 0xFF;
                yield return ((accumulator ^ 0x80) - 0x80) / 128f;
            }
        }
    }
}
