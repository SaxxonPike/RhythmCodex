using System.Collections.Generic;
using System.Linq;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;

namespace RhythmCodex.Djmain.Converters
{
    [Service]
    public class DjmainAudioDecoder : IDjmainAudioDecoder
    {
        private static readonly int[] DpcmTable =
        {
            0x00, 0x01, 0x02, 0x04,
            0x08, 0x10, 0x20, 0x40,
            0x00, 0xC0, 0xE0, 0xF0,
            0xF8, 0xFC, 0xFE, 0xFF
        };

        public IList<float> DecodeDpcm(IEnumerable<byte> data)
        {
            return DecodeDpcmData(data).ToArray();
        }

        public IList<float> DecodePcm8(IEnumerable<byte> data)
        {
            return data.Select(b => ((b ^ 0x80) - 0x80) / 128f).ToArray();
        }

        public IList<float> DecodePcm16(IEnumerable<byte> data)
        {
            return DecodePcm16Data(data).ToArray();
        }

        private static IEnumerable<float> DecodePcm16Data(IEnumerable<byte> data)
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

        private static IEnumerable<float> DecodeDpcmData(IEnumerable<byte> data)
        {
            var accumulator = 0x00;
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