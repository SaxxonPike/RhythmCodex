using System.Collections.Generic;
using System.IO;
using System.Linq;
using RhythmCodex.Beatmania.Models;
using RhythmCodex.IoC;

namespace RhythmCodex.Beatmania.Streamers;

[Service]
public class BeatmaniaPc1StreamWriter : IBeatmaniaPc1StreamWriter
{
    public void Write(Stream target, IEnumerable<BeatmaniaPc1Chart> charts)
    {
        var offsetTable = new int[12];
        var lengthTable = new int[12];
        using var buffer = new MemoryStream();
        var bufferWriter = new BinaryWriter(buffer);
        foreach (var chart in charts.Where(c => c?.Data != null))
        {
            if (chart.Index is < 0 or >= 12)
                continue;

            lengthTable[chart.Index] = chart.Data.Count * 4 + 4;
            offsetTable[chart.Index] = (int) (buffer.Position + 0x60);
            foreach (var ev in chart.Data)
            {
                bufferWriter.Write(ev.LinearOffset);
                bufferWriter.Write(ev.Parameter0);
                bufferWriter.Write(ev.Parameter1);
                bufferWriter.Write(ev.Value);
            }
            bufferWriter.Write(0x7FFFFFFF);
            bufferWriter.Write(0x00000000);
        }

        bufferWriter.Flush();
        buffer.Position = 0;
        var writer = new BinaryWriter(target);
        for (var i = 0; i < 12; i++)
        {
            writer.Write(offsetTable[i]);
            writer.Write(lengthTable[i]);
        }
        writer.Flush();
        buffer.CopyTo(target);
        target.Flush();
    }
}