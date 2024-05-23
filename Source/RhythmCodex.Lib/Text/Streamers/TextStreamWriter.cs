using System.Collections.Generic;
using System.IO;
using RhythmCodex.IoC;

namespace RhythmCodex.Text.Streamers;

[Service]
public class TextStreamWriter : ITextStreamWriter
{
    public void Write(Stream stream, IEnumerable<string> lines)
    {
        var writer = new StreamWriter(stream);
        foreach (var line in lines)
            writer.WriteLine(line);
    }
}