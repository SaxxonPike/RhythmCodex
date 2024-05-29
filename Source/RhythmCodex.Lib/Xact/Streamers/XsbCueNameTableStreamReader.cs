using System.Collections.Generic;
using System.IO;
using System.Linq;
using RhythmCodex.Extensions;
using RhythmCodex.IoC;

namespace RhythmCodex.Xact.Streamers;

[Service]
public class XsbCueNameTableStreamReader : IXsbCueNameTableStreamReader
{
    public IEnumerable<string> Read(Stream stream, int length)
    {
        var result = new List<string>();
        var reader = new BinaryReader(stream);
        var buffer = new List<byte>();
        var remaining = length;

        while (remaining-- > 0)
        {
            var b = reader.ReadByte();
            if (b == 0)
            {
                result.Add(buffer.ToArray().GetString());
                buffer.Clear();
                continue;
            }
                
            buffer.Add(b);
        }
            
        if (buffer.Count != 0)
            result.Add(buffer.ToArray().GetString());

        return result;
    }
}