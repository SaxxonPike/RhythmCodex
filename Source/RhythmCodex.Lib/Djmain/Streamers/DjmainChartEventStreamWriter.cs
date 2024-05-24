using System.Collections.Generic;
using System.IO;
using RhythmCodex.Djmain.Model;
using RhythmCodex.IoC;

namespace RhythmCodex.Djmain.Streamers;

[Service]
public class DjmainChartEventStreamWriter : IDjmainChartEventStreamWriter
{
    public int Write(Stream stream, IEnumerable<DjmainChartEvent> events)
    {
        var writer = new BinaryWriter(stream);
        var size = 4;

        foreach (var ev in events)
        {
            writer.Write(ev.Offset);
            writer.Write(ev.Param0);
            writer.Write(ev.Param1);
            size += 4;
        }

        writer.Write(0x7FFF);
        return size;
    }
}