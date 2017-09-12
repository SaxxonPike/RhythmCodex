using System.Collections.Generic;
using System.IO;
using System.Linq;
using RhythmCodex.Djmain.Model;
using RhythmCodex.Infrastructure;

namespace RhythmCodex.Djmain.Streamers
{
    [Service]
    public class DjmainChartEventStreamReader : IDjmainChartEventStreamReader
    {
        private const int MaxEventCount = 0x1000;

        public IList<DjmainChartEvent> Read(Stream stream)
        {
            return ReadInternal(stream).ToArray();
        }

        private static IEnumerable<DjmainChartEvent> ReadInternal(Stream stream)
        {
            var reader = new BinaryReader(stream);

            for (var i = 0; i < MaxEventCount; i++)
            {
                var result = new DjmainChartEvent
                {
                    Offset = reader.ReadUInt16(),
                    Param0 = reader.ReadByte(),
                    Param1 = reader.ReadByte()
                };

                if (result.Offset == 0x7FFF)
                    yield break;

                yield return result;
            }
        }
    }
}
