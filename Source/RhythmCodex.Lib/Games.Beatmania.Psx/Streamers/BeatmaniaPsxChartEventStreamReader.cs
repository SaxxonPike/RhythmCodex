using System.Collections.Generic;
using System.IO;
using System.Linq;
using RhythmCodex.Archs.Djmain.Model;
using RhythmCodex.IoC;
using RhythmCodex.Utils.Cursors;

namespace RhythmCodex.Games.Beatmania.Psx.Streamers;

[Service]
public class BeatmaniaPsxChartEventStreamReader : IBeatmaniaPsxChartEventStreamReader
{
    public List<DjmainChartEvent> Read(Stream stream, int length)
    {
        return ReadInternal(stream, length).ToList();
    }

    private static IEnumerable<DjmainChartEvent> ReadInternal(Stream stream, int length)
    {
        for (var i = 0; i < length / 4; i++)
        {
            stream
                .ReadU16L(out var offset)
                .ReadU8(out var param0)
                .ReadU8(out var param1);

            if (offset == 0x7FFF)
                yield break;

            var isSound = (param0 & 0xF) == 0x1 || (param0 & 0xF) == 0x5;

            if (isSound)
            {
                if ((param1 & 0x80) == 0)
                    continue;

                param1 &= 0x7F;
            }

            yield return new DjmainChartEvent
            {
                Offset = offset,
                Param0 = param0,
                Param1 = param1
            };
        }
    }
}