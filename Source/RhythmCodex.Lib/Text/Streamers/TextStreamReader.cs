using System.Collections.Generic;
using System.IO;
using System.Linq;
using RhythmCodex.IoC;

namespace RhythmCodex.Text.Streamers;

[Service]
public class TextStreamReader : ITextStreamReader
{
    public List<string> Read(Stream stream)
    {
        return ReadInternal(stream).ToList();
    }

    private IEnumerable<string> ReadInternal(Stream stream)
    {
        var reader = new StreamReader(stream);
        while (true)
        {
            var line = reader.ReadLine();
            if (line == null)
                yield break;
            yield return line;
        }
    }
}