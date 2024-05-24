using System.Collections.Generic;
using System.IO;
using System.Linq;
using RhythmCodex.Djmain.Model;
using RhythmCodex.IoC;

namespace RhythmCodex.Beatmania.Streamers;

[Service]
public class BeatmaniaPsxChartEventStreamReader : IBeatmaniaPsxChartEventStreamReader
{
    public List<DjmainChartEvent> Read(Stream stream, int length)
    {
        return ReadInternal(stream, length).ToList();
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
                yield break;

            var isSound = (result.Param0 & 0xF) == 0x1 || (result.Param0 & 0xF) == 0x5;

            if (isSound)
            {
                if ((result.Param1 & 0x80) == 0)
                    continue;

                result.Param1 &= 0x7F;
            }

            yield return result;
        }
    }
}