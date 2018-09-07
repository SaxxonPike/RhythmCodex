using System.Collections.Generic;
using System.IO;
using System.Linq;
using RhythmCodex.Djmain.Model;
using RhythmCodex.Infrastructure;

namespace RhythmCodex.BeatmaniaPsx.Streamers
{
    [Service]
    public class BeatmaniaPsxChartEventStreamReader
    {
        public IList<DjmainChartEvent> Read(Stream stream, int length)
        {
            return ReadInternal(stream, length).ToArray();
        }

        private static IEnumerable<DjmainChartEvent> ReadInternal(Stream stream, int length)
        {
            var reader = new BinaryReader(stream);

            for (var i = 0; i < length / 4; i++)
            {
                var result = new DjmainChartEvent
                {
                    Offset = reader.ReadUInt16(),
                    Param0 = reader.ReadByte(),
                    Param1 = reader.ReadByte()
                };

                if (result.Offset == 0x7FFF)
                    continue;

                yield return result;
            }
        }
    }
}